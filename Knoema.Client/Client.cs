using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Knoema.Data;
using Knoema.Meta;
using Newtonsoft.Json;
using Knoema.Upload;

namespace Knoema
{
	public class Client
	{
		string _host;
		string _appId;
		string _appSecret;
		string _token;

		const string AuthProtoVersion = "1.2";

		public Client(string host)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			_host = host;
		}

		public Client(string host, string token)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			if (string.IsNullOrEmpty(token))
				throw new ArgumentNullException("token");

			_host = host;
			_token = token;
		}

		public Client(string host, string appId, string appSecret)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			if (string.IsNullOrEmpty(appId))
				throw new ArgumentNullException("appId");

			if (string.IsNullOrEmpty(appSecret))
				throw new ArgumentNullException("appSecret");

			_host = host;
			_appId = appId;
			_appSecret = appSecret;
		}

		private HttpClient GetApiClient()
		{
			var client = new HttpClient();

			if (!string.IsNullOrEmpty(_appId) && !string.IsNullOrEmpty(_appSecret))
				client.DefaultRequestHeaders.Add("Authorization",
					string.Format("Knoema {0}:{1}:{2}", _appId,
						Convert.ToBase64String(
							new HMACSHA1(
								Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("dd-MM-yy-HH"))).ComputeHash(Encoding.UTF8.GetBytes(_appSecret))),
								AuthProtoVersion
					)
				);

			return client;
		}

		private Task<T> ApiGet<T>(string path, string query = null)
		{
			var builder = new UriBuilder(Uri.UriSchemeHttp, _host);

			if (!string.IsNullOrEmpty(path))
				builder.Path = path;

			if (!string.IsNullOrEmpty(_token))
				query = query + "&access_token=" + _token;

			if (!string.IsNullOrEmpty(query))
				builder.Query = query;

			return GetApiClient().GetStringAsync(builder.Uri).Then((resp) => JsonConvert.DeserializeObjectAsync<T>(resp));
		}

		private Task<T> ApiPost<T>(string path, HttpContent content)
		{
			var builder = new UriBuilder(Uri.UriSchemeHttp, _host);
			builder.Path = path;

			if (!string.IsNullOrEmpty(_token))
				builder.Query = "access_token=" + _token;

			return GetApiClient().PostAsync(builder.Uri, content).Then((resp) => resp.Content.ReadAsStringAsync())
				.Then((resp) => JsonConvert.DeserializeObjectAsync<T>(resp));
		}

		private Task<T> ApiPost<T>(string path, object obj)
		{
			var content = new StringContent(JsonConvert.SerializeObject(obj));
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			return ApiPost<T>(path, content);
		}

		public Task<IEnumerable<Dataset>> ListDatasets(string source = null, string topic = null, string region = null)
		{
			if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(topic) && string.IsNullOrEmpty(region))
				return ApiGet<IEnumerable<Dataset>>("/api/1.0/meta/dataset");

			return ApiPost<IEnumerable<Dataset>>("/api/1.0/meta/dataset", new Dictionary<string, string>()
					{
						{"source", source},
						{"topic", topic},
						{"region", region}
					});
		}

		public Task<Dataset> GetDataset(string id)
		{
			return ApiGet<Dataset>(string.Format("/api/1.0/meta/dataset/{0}", id));
		}

		public Task<Dimension> GetDatasetDimension(string dataset, string dimension)
		{
			return ApiGet<Dimension>(string.Format("/api/1.0/meta/dataset/{0}/dimension/{1}", dataset, dimension));
		}

		public Task<PivotResponse> GetData(PivotRequest pivot)
		{
			return ApiPost<PivotResponse>("/api/1.0/data/pivot/", pivot);
		}

		public Task<PostResult> UploadPost(string fileName)
		{
			var fi = new FileInfo(fileName);
			var fs = fi.OpenRead();
			var form = new MultipartFormDataContent();
			form.Add(new StreamContent(fs), "\"file\"", "\"" + fi.Name + "\"");
			return ApiPost<PostResult>("/api/1.0/upload/post", form);
		}

		public Task<VerifyResult> UploadVerify(string filePath, string existingDatasetIdToModify = null)
		{
			return ApiGet<VerifyResult>("/api/1.0/upload/verify", string.Format("filePath={0}&datasetId={1}", filePath, existingDatasetIdToModify));
		}

		public Task<UploadResult> UploadSubmit(DatasetUpload upload)
		{
			return ApiPost<UploadResult>("/api/1.0/upload/save", upload);
		}

		public Task<UploadResult> UploadStatus(int uploadId)
		{
			return ApiGet<UploadResult>("/api/1.0/upload/status", string.Format("id={0}", uploadId));
		}

		public Task<UploadResult> UploadDataset(string filename, string datasetName)
		{
			var postResult = UploadPost(filename).Result;
			if (!postResult.Successful)
				return null;

			var verifyResult = UploadVerify(postResult.Properties.Location).Result;
			if (!verifyResult.Successful)
				return null;

			var upload = new DatasetUpload()
			{
				Name = datasetName,
				UploadFormatType = verifyResult.UploadFormatType,
				Columns = verifyResult.Columns,
				FlatDSUpdateOptions = verifyResult.FlatDSUpdateOptions,
				FileProperty = postResult.Properties
			};

			var result = UploadSubmit(upload).Result;
			while (UploadStatus(result.Id).Result.Status == "in progress")
			{
				System.Threading.Thread.Sleep(5000);
			}

			return UploadStatus(result.Id);
		}

	}
}
