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
using Knoema.Upload;
using Newtonsoft.Json;

namespace Knoema
{
	public class Client
	{
		string _appId;
		string _appSecret;
		Uri _uri;
		public CookieCollection cookies { get; set; }


		public Client(string host)
		{
			Uri.TryCreate(string.Format("http://{0}", host), UriKind.Absolute, out _uri);
		}

		public Client(string host, string appId, string appSecret)
		{
			Uri.TryCreate(string.Format("http://{0}", host), UriKind.Absolute, out _uri);
			_appId = appId;
			_appSecret = appSecret;
		}

		public Task<Dataset> GetDataset(string id)
		{
			return JsonConvert.DeserializeObjectAsync<Dataset>(
				DataRequest(string.Format("{0}api/1.0/meta/dataset/{1}", _uri.AbsoluteUri, id)).Result
			);
		}

		public Task<Dimension> GetDatasetDimension(string dataset, string dimension)
		{
			return JsonConvert.DeserializeObjectAsync<Dimension>(
				DataRequest(
					string.Format("{0}api/1.0/meta/dataset/{1}/dimension/{2}", _uri.AbsoluteUri, dataset, dimension)).Result
			);
		}

		public Task<PivotResponse> GetData(PivotRequest pivot)
		{
			return JsonConvert.DeserializeObjectAsync<PivotResponse>(
				DataRequest(
					string.Format("{0}api/1.0/data/pivot/", _uri.AbsoluteUri), JsonConvert.SerializeObject(pivot)).Result
			);
		}

		Task<string> DataRequest(string url, string data = null)
		{
			var client = new HttpClient();

			if (!string.IsNullOrEmpty(_appId) && !string.IsNullOrEmpty(_appSecret))
				client.DefaultRequestHeaders.Add("Authorization",
					string.Format("Knoema {0}:{1}", _appId, 
						Convert.ToBase64String(
							new HMACSHA1(
								Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("dd-mm-yy-hh"))).ComputeHash(Encoding.UTF8.GetBytes(_appSecret)))
					)
				);

			if (!string.IsNullOrEmpty(data))
			{
				var content = new StringContent(data);			
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				return client.PostAsync(url, content).Result.Content.ReadAsStringAsync();
			}

			return client.GetStringAsync(url);
		}

		public async Task<UploadResult> UploadDataset(string filename, string datasetName, string datasetId = null)
		{
			var result = new UploadResult();
			var cookieJar = new CookieContainer();
			var handler = new HttpClientHandler() { CookieContainer = cookieJar };
			cookieJar.Add(cookies);
			handler.UseCookies = true;
			handler.UseDefaultCredentials = false;

			try
			{
				var postResult = await (JsonConvert.DeserializeObjectAsync<PostResult>(PostFile(filename, handler).Result));
				if (!postResult.Successful)
				{
					result.Status = "Failed";
					result.Errors.Add(postResult.Error);
					return result;
				}

				var verifyResult = await (JsonConvert.DeserializeObjectAsync<VerifyResult>
					((JsonConvert.DeserializeObject(VerifyFile(postResult.Properties.Location, datasetId, handler).Result)).ToString()
					, new JsonSerializerSettings()
							{
								NullValueHandling = NullValueHandling.Ignore
							}));

				if (!verifyResult.Successful)
				{
					result.Status = "Failed";
					result.Errors = verifyResult.ErrorList;
					return result;
				}

				var datasetupload = new DatasetUpload()
				{
					Name = datasetName,
					DatasetId = datasetId,
					UploadFormatType = verifyResult.UploadFormatType,
					Columns = verifyResult.Columns,
					FlatDSUpdateOptions = verifyResult.FlatDSUpdateOptions,
					FileProperty = postResult.Properties
				};

				result = await JsonConvert.DeserializeObjectAsync<UploadResult>(SubmitFile(JsonConvert.SerializeObject(datasetupload), handler).Result);
				result = await GetReport(result, handler);
			}
			catch(Exception ex)
			{
				result.Errors.Add("Error in upload due to:" + ex.Message);
			}
			return result;
		}

		async Task<UploadResult> GetReport(UploadResult result, HttpClientHandler handler)
		{
			var url = new Uri(string.Format("{0}api/1.0/upload/status?id={1}&client_id={2}", _uri.AbsoluteUri, result.Id, _appId));
			do
			{
				var client = new HttpClient(handler as HttpMessageHandler) { BaseAddress = url };
				result = await (JsonConvert.DeserializeObjectAsync<UploadResult>((client.GetAsync(url).Result.Content.ReadAsStringAsync()).Result));
			} while (result.Status == "In Progress");

			return result;
		}

		Task<string> PostFile(string fileName, HttpClientHandler handler)
		{
			var postUrl = new Uri(string.Format("{0}api/1.0/upload/post/", _uri.AbsoluteUri));
			var fi = new FileInfo(fileName);
			var form = new MultipartFormDataContent();
			form.Add(new StreamContent(fi.OpenRead()), "\"file\"", "\"" + fi.Name + "\"");
			var client = new HttpClient(handler as HttpMessageHandler) { BaseAddress = postUrl };

			return client.PostAsync(postUrl, form).Result.Content.ReadAsStringAsync();
		}

		Task<string> VerifyFile(string fileName, string datasetId, HttpClientHandler handler)
		{
			var url = new Uri(string.Format("{0}api/1.0/upload/verify?filePath={1}&datasetId={2}", _uri.AbsoluteUri, fileName, datasetId));

			var client = new HttpClient(handler as HttpMessageHandler) { BaseAddress = url };

			return client.GetAsync(url).Result.Content.ReadAsStringAsync();
		}

		Task<string> SubmitFile(string data, HttpClientHandler handler)
		{
			var postUrl = new Uri(string.Format("{0}api/1.0/upload/save/", _uri.AbsoluteUri));
			var content = new StringContent(data);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var client = new HttpClient(handler as HttpMessageHandler) { BaseAddress = postUrl };

			return client.PostAsync(postUrl, content).Result.Content.ReadAsStringAsync();
		}

		public async Task<CookieCollection> Login(string userName, string password)
		{
			Uri loginUrl = new Uri(string.Format("{0}sys/login", _uri.AbsoluteUri));
			CookieContainer cookieJar = new CookieContainer();
			HttpClientHandler handler = new HttpClientHandler()
			{
				CookieContainer = cookieJar
			};
			handler.UseCookies = true;
			handler.UseDefaultCredentials = false;
			HttpClient client = new HttpClient(handler as HttpMessageHandler) { BaseAddress = loginUrl };

			HttpContent content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("EMail", userName),
				new KeyValuePair<string, string>("Password", password),
			});

			content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
			var response = await client.PostAsync(loginUrl, content);
			return cookieJar.GetCookies(loginUrl);
		}
	}
}
