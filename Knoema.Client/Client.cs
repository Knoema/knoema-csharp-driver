using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Knoema.Data;
using Knoema.Meta;
using Knoema.Search;
using Knoema.Upload;
using Newtonsoft.Json;

namespace Knoema
{
	public class Client
	{
		private const string _apiDataPivot = "/api/1.0/data/pivot/";
		private const string _api11DataPivot = "/api/1.1/data/pivot/";
		private const string _apiDataMultipivot = "/api/1.0/data/multipivot";
		private const string _apiMetaDataset = "/api/1.0/meta/dataset/{0}";
		private const string _apiMetaDatasetList = "/api/1.0/meta/dataset";
		private const string _apiMetaDatasetDimension = "/api/1.0/meta/dataset/{0}/dimension/{1}";
		private const string _apiMetaDatasetDateRange = "/api/1.0/meta/dataset/{0}/daterange";
		private const string _apiMetaVerifyDataset = "/api/1.0/meta/verifydataset";
		private const string _apiUploadPost = "/api/1.0/upload/post";
		private const string _apiUploadVerify = "/api/1.0/upload/verify";
		private const string _apiUploadSave = "/api/1.0/upload/save";
		private const string _apiUploadStatus = "/api/1.0/upload/status";
		private const string _apiDataRaw = "/api/1.0/data/raw/";
		private const string _apiSearch = "/api/1.0/search";
		private const string _apiSearchConfig = "/api/1.0/search/config";

		private readonly string _host;
		private readonly string _appId;
		private readonly string _appSecret;
		private readonly string _token;

		private const string _authProtoVersion = "1.2";
		private const int _httpClientTimeout = 300 * 1000;

		public Client(string host)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			_host = host;
		}

		public Client(string host, string token)
			: this(host)
		{
			if (string.IsNullOrEmpty(token))
				throw new ArgumentNullException("token");

			_token = token;
		}

		public Client(string host, string appId, string appSecret)
			: this(host)
		{
			if (string.IsNullOrEmpty(appId))
				throw new ArgumentNullException("appId");

			if (string.IsNullOrEmpty(appSecret))
				throw new ArgumentNullException("appSecret");

			_appId = appId;
			_appSecret = appSecret;
		}

		private HttpClient GetApiClient()
		{
			var clientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
			return new HttpClient(clientHandler) { Timeout = TimeSpan.FromMilliseconds(_httpClientTimeout) };
		}

		private static Uri GetUri(string host, string token, string path, Dictionary<string, string> parameters = null)
		{
			if (!string.IsNullOrEmpty(token))
			{
				if (parameters == null)
					parameters = new Dictionary<string, string>();
				parameters.Add("access_token", token);
			}
			var builder = new UriBuilder(Uri.UriSchemeHttp, host)
			{
				Path = path,
				Query = parameters != null ?
					string.Join("&", parameters.Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value)))) :
					string.Empty
			};
			return builder.Uri;
		}

		private Uri GetDataUri(string path, Dictionary<string, string> parameters = null)
		{
			return GetUri(_host, _token, path, parameters);
		}

		private Task<T> ApiGet<T>(string path, Dictionary<string, string> parameters = null)
		{
			var message = new HttpRequestMessage(HttpMethod.Get, GetDataUri(path, parameters));
			return ApiAccess<T>(message);
		}

		private Task<TResponse> ApiPost<TRequest, TResponse>(string path, TRequest request, Dictionary<string, string> parameters = null)
		{
			var message = new HttpRequestMessage(HttpMethod.Post, GetDataUri(path, parameters))
			{
				Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
			};
			return ApiAccess<TResponse>(message);
		}

		private async Task<TResponse> ApiAccess<TResponse>(HttpRequestMessage message)
		{
			if (!string.IsNullOrEmpty(_appId) && !string.IsNullOrEmpty(_appSecret))
				message.Headers.Add("Authorization", GetAuthorizationHeaderValue(_appId, _appSecret));

			var sendAsyncResp = await GetApiClient().SendAsync(message);
			sendAsyncResp.EnsureSuccessStatusCode();
			var strRead = await sendAsyncResp.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<TResponse>(strRead);
		}

		private static string GetAuthorizationHeaderValue(string appId, string appSecret)
		{
			return string.Format("Knoema {0}:{1}:{2}", appId,
					Convert.ToBase64String(new HMACSHA1(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("dd-MM-yy-HH"))).ComputeHash(Encoding.UTF8.GetBytes(appSecret))),
					_authProtoVersion);
		}

		public Task<Dataset> GetDataset(string datasetId)
		{
			return ApiGet<Dataset>(string.Format(_apiMetaDataset, datasetId));
		}

		public Task<Dimension> GetDatasetDimension(string datasetId, string dimensionId)
		{
			return ApiGet<Dimension>(string.Format(_apiMetaDatasetDimension, datasetId, dimensionId));
		}

		public Task<List<Dataset>> ListDatasets(string source = null, string topic = null, string region = null)
		{
			if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(topic) && string.IsNullOrEmpty(region))
				return ApiGet<List<Dataset>>(_apiMetaDatasetList);

			return ApiPost<Dictionary<string, string>, List<Dataset>>(_apiMetaDatasetList,
				new Dictionary<string, string>()
					{
						{"source", source},
						{"topic", topic},
						{"region", region}
					});
		}

		public Task<PivotResponse> GetData(PivotRequest pivot)
		{
			return ApiPost<PivotRequest, PivotResponse>(_apiDataPivot, pivot);
		}

		public Task<List<PivotResponse>> GetData(List<PivotRequest> pivots)
		{
			return ApiPost<List<PivotRequest>, List<PivotResponse>>(_apiDataMultipivot, pivots);
		}

		public Task<RegularTimeSeriesRawDataResponse> GetDataBegin(PivotRequest pivot)
		{
			return ApiPost<PivotRequest, RegularTimeSeriesRawDataResponse>(_apiDataRaw, pivot);
		}

		public Task<RegularTimeSeriesRawDataResponse> GetDataStreaming(string token)
		{
			var parameters = new Dictionary<string, string>();
			parameters.Add("continuationToken", token);
			return ApiGet<RegularTimeSeriesRawDataResponse>(_apiDataRaw, parameters);
		}

		public Task<FlatTimeSeriesRawDataResponse> GetFlatDataBegin(PivotRequest pivot)
		{
			return ApiPost<PivotRequest, FlatTimeSeriesRawDataResponse>(_apiDataRaw, pivot);
		}

		public Task<FlatTimeSeriesRawDataResponse> GetFlatDataStreaming(string token)
		{
			var parameters = new Dictionary<string, string>();
			parameters.Add("continuationToken", token);
			return ApiGet<FlatTimeSeriesRawDataResponse>(_apiDataRaw, parameters);
		}

		public Task<PostResult> UploadPost(string fileName)
		{
			var fileInfo = new FileInfo(fileName);
			var fileStream = fileInfo.OpenRead();

			var content = new MultipartFormDataContent();
			content.Add(new StreamContent(fileStream), "\"file\"", "\"" + fileInfo.Name + "\"");

			var uri = GetDataUri(_apiUploadPost);
			var message = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };
			return ApiAccess<PostResult>(message);

		}

		public Task<VerifyResult> UploadVerify(string filePath, string existingDatasetIdToModify = null)
		{
			var parameters = new Dictionary<string, string>();
			parameters.Add("filePath", filePath);
			if (existingDatasetIdToModify != null)
				parameters.Add("datasetId", existingDatasetIdToModify);
			return ApiGet<VerifyResult>(_apiUploadVerify, parameters);
		}

		public Task<UploadResult> UploadSubmit(DatasetUpload upload)
		{
			return ApiPost<DatasetUpload, UploadResult>(_apiUploadSave, upload);
		}

		public Task<UploadResult> UploadStatus(int uploadId)
		{
			var parameters = new Dictionary<string, string>();
			parameters.Add("id", uploadId.ToString());
			return ApiGet<UploadResult>(_apiUploadStatus, parameters);
		}

		public Task<VerifyDatasetResult> VerifyDataset(string id, DateTime? publicationDate = null, string source = null, string refUrl = null)
		{
			var request = new VerifyDatasetRequest
			{
				Id = id,
				PublicationDate = publicationDate,
				Source = source,
				RefUrl = refUrl
			};
			return ApiPost<VerifyDatasetRequest, VerifyDatasetResult>(_apiMetaVerifyDataset, request);
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
				System.Threading.Thread.Sleep(5000);

			return UploadStatus(result.Id);
		}

		public Task<DateRange> GetDatasetDateRange(string datasetId)
		{
			return ApiGet<DateRange>(string.Format(_apiMetaDatasetDateRange, datasetId));
		}

		public async Task<SearchTimeSeriesResponse> Search(string searchText, SearchScope scope, int count, int version)
		{
			var parameters = new Dictionary<string, string>();
			parameters.Add("query", searchText.Trim());
			parameters.Add("scope", scope.GetString());
			parameters.Add("count", count.ToString());
			parameters.Add("version", version.ToString());

			var configResponse = await ApiGet<ConfigResponse>(_apiSearchConfig);
			var searchHost = configResponse.SearchHost;
			var message = new HttpRequestMessage(HttpMethod.Post, GetUri(searchHost, _token, _apiSearch, parameters));
			return await ApiAccess<SearchTimeSeriesResponse>(message);
		}
	}
}
