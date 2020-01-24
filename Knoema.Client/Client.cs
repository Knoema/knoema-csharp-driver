using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Knoema.Data;
using Knoema.Meta;
using Knoema.Search;
using Knoema.Search.TimeseriesSearch;
using Knoema.Upload;

using Newtonsoft.Json;

namespace Knoema
{
	public class Client
	{
		private readonly string _scheme;
		private readonly string _host;
		private readonly string _clientId;
		private readonly string _clientSecret;
		private readonly string _token;
		private readonly bool _ignoreCertErrors;

		private string _searchHost;
		private string _searchCommunityId;

		private CookieContainer _cookies = new CookieContainer();
		private HttpClient _client;

		private const string AuthProtoVersion = "1.2";
		private const int DefaultHttpTimeout = 600 * 1000;

		public int HttpTimeout { get; set; }

		private Client(bool ignoreCertErrors, string scheme)
		{
			HttpTimeout = DefaultHttpTimeout;

			_scheme = scheme;
			if (string.IsNullOrEmpty(_scheme))
				_scheme = Uri.UriSchemeHttp;

			_ignoreCertErrors = ignoreCertErrors;
		}

		public Client(string host, bool ignoreCertErrors = false, string scheme = "")
			: this(ignoreCertErrors, scheme)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			_host = host;
		}

		public Client(string host, string token, bool ignoreCertErrors = false, string scheme = "")
			: this(ignoreCertErrors, scheme)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			if (string.IsNullOrEmpty(token))
				throw new ArgumentNullException("token");

			_host = host;
			_token = token;
		}

		public Client(string host, string clientId, string clientSecret, bool ignoreCertErrors = false, string scheme = "")
			: this(ignoreCertErrors, scheme)
		{
			if (string.IsNullOrEmpty(host))
				throw new ArgumentNullException("host");

			if (string.IsNullOrEmpty(clientId))
				throw new ArgumentNullException("clientId");

			if (clientSecret == null)
				throw new ArgumentNullException("clientSecret");

			_host = host;
			_clientId = clientId;
			_clientSecret = clientSecret;
		}

		private HttpClient GetApiClient()
		{
			if (_client == null)
			{
#if NET45
				var clientHandler = new WebRequestHandler();
#else
				var clientHandler = new HttpClientHandler();
#endif

				if (_ignoreCertErrors)
#if NET45
					clientHandler.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
#else
					clientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
#endif

				clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				clientHandler.CookieContainer = _cookies;

				_client = new HttpClient(clientHandler) { Timeout = TimeSpan.FromMilliseconds(HttpTimeout) };
			}

			if (!string.IsNullOrEmpty(_clientId) && !string.IsNullOrEmpty(_clientSecret))
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", GetAuthorizationHeaderValue(_clientId, _clientSecret));
						
			return _client;
		}

		private Uri MakeUri(string path, string query = null)
		{
			var builder = new UriBuilder(_scheme, _host);

			if (!string.IsNullOrEmpty(path))
				builder.Path = path;

			if (!string.IsNullOrEmpty(_token))
			{
				if (!string.IsNullOrEmpty(query))
					query += '&';
				query += "access_token=" + _token;
			}

			if (!string.IsNullOrEmpty(_clientId) && string.IsNullOrEmpty(_clientSecret))
			{
				if (!string.IsNullOrEmpty(query))
					query += '&';
				query += "client_id=" + _clientId;
			}

			if (!string.IsNullOrEmpty(query))
				builder.Query = query;

			return builder.Uri;
		}

		private async Task<T> ApiGet<T>(string path, string query = null)
		{
			var response = await GetApiClient().GetAsync(MakeUri(path, query));
			EnsureSuccessApiCall(response);
			var content = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(content);
		}

		private async Task<T> ApiPost<T>(string path, HttpContent content)
		{
			var postResponse = await GetApiClient().PostAsync(MakeUri(path, null), content);
			EnsureSuccessApiCall(postResponse);
			var readString = await postResponse.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(readString);
		}

		private static void EnsureSuccessApiCall(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				var error = "";
				if (response.Content != null)
				{
					error = response.Content.ReadAsStringAsync().Result;
					error = Regex.Replace(error, "<style>(.|\n)+?</style>|<[^>]+>", string.Empty, RegexOptions.Multiline);
					error = Regex.Replace(error, @"\r\n\s*\r\n", "\r\n").Trim();
				}
				var statusCode = (int)response.StatusCode;

				throw new WebException(
					string.Format("Remote server returned error {0}{1}",
						statusCode,
						string.IsNullOrEmpty(error)
							? string.Empty
							: string.Format("{0}{0}{1}", Environment.NewLine, error)));
			}
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

		public Task<List<PivotResponse>> GetData(List<PivotRequest> pivots)
		{
			return ApiPost<List<PivotResponse>>("/api/1.0/data/multipivot", pivots);
		}

		public Task<RegularTimeSeriesRawDataResponse> GetDataBegin(PivotRequest pivot)
		{
			return ApiPost<RegularTimeSeriesRawDataResponse>("/api/1.0/data/raw/", pivot);
		}

		public Task<RegularTimeSeriesRawDataResponse> GetDataStreaming(string token)
		{
			return ApiGet<RegularTimeSeriesRawDataResponse>("/api/1.0/data/raw/", string.Format("continuationToken={0}", token));
		}

		public Task<FlatTimeSeriesRawDataResponse> GetFlatDataBegin(PivotRequest pivot)
		{
			return ApiPost<FlatTimeSeriesRawDataResponse>("/api/1.0/data/raw/", pivot);
		}

		public Task<FlatTimeSeriesRawDataResponse> GetFlatDataStreaming(string token)
		{
			return ApiGet<FlatTimeSeriesRawDataResponse>("/api/1.0/data/raw/", string.Format("continuationToken={0}", token));
		}

		public Task<IEnumerable<UnitMember>> GetUnits()
		{
			return ApiGet<IEnumerable<UnitMember>>("/api/1.0/meta/units");
		}

		public Task<IEnumerable<TimeSeriesItem>> GetTimeSeriesList(string datasetId, FullDimensionRequest request)
		{
			return ApiPost<IEnumerable<TimeSeriesItem>>(string.Format("/api/1.0/data/dataset/{0}", datasetId), request);
		}

		public async Task<PostResult> UploadPost(string fileName)
		{
			var fi = new FileInfo(fileName);
			using (var fs = fi.OpenRead())
			{
				var form = new MultipartFormDataContent();
				using (var streamContent = new StreamContent(fs))
				{
					form.Add(streamContent, "\"file\"", "\"" + fi.Name + "\"");
					return await ApiPost<PostResult>("/api/1.0/upload/post", form);
				}
			}
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

		public Task<VerifyDatasetResult> UpdateDatasetMetadata(string datasetId, MetadataUpdate metadataUpdate)
		{
			return ApiPost<VerifyDatasetResult>(string.Format("/api/1.0/meta/dataset/{0}", datasetId), metadataUpdate);
		}

		public Task<VerifyDatasetResult> VerifyDataset(string id, DateTime? publicationDate = null, string source = null, string refUrl = null, DateTime? nextReleaseDate = null)
		{
			return ApiPost<VerifyDatasetResult>("/api/1.0/meta/verifydataset", new
			{
				id = id,
				publicationDate = publicationDate,
				source = source,
				refUrl = refUrl,
				nextReleaseDate = nextReleaseDate
			});
		}

		public Task<DateRange> GetDatasetDateRange(string datasetId)
		{
			return ApiGet<DateRange>(string.Format("/api/1.0/meta/dataset/{0}/daterange", datasetId));
		}

		public async Task<Response> SearchTimeseries(Request request, string lang = null)
		{
			if (_searchHost == null)
			{
				var configResponse = await ApiGet<ConfigResponse>("/api/1.0/search/config");
				_searchHost = configResponse.SearchHost;
				_searchCommunityId = configResponse.CommunityId;
			}

			var parameters = new Dictionary<string, string>
			{
				{ "host", _host },
				{ "baseHost", _host }
			};
			if (!string.IsNullOrEmpty(_searchCommunityId))
				parameters.Add("communityId", _searchCommunityId);
			if (lang != null)
				parameters.Add("lang", lang);


			var message = new HttpRequestMessage(HttpMethod.Post, GetUri(_searchHost, _token, "/api/1.0/search/timeseries", _scheme, parameters));

			if (!string.IsNullOrEmpty(_clientId) && !string.IsNullOrEmpty(_clientSecret))
				message.Headers.Add("Authorization", GetAuthorizationHeaderValue(_clientId, _clientSecret));

			var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

			message.Content = content;

			var sendAsyncResp = await GetApiClient().SendAsync(message);
			sendAsyncResp.EnsureSuccessStatusCode();
			var strRead = await sendAsyncResp.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<Response>(strRead);
			foreach (var datasetItem in result.Items)
				foreach (var series in datasetItem.Items)
					series.Dataset = datasetItem.Dataset;
			return result;
		}

		public async Task<SearchTimeSeriesResponse> Search(string searchText, SearchScope scope, int count, int version, string lang = null)
		{
			if (_searchHost == null)
			{
				var configResponse = await ApiGet<ConfigResponse>("/api/1.0/search/config");
				_searchHost = configResponse.SearchHost;
				_searchCommunityId = configResponse.CommunityId;
			}

			var parameters = new Dictionary<string, string>
			{
				{ "query", searchText.Trim() },
				{ "scope", scope.GetString() },
				{ "count", count.ToString() },
				{ "version", version.ToString() },
				{ "host", _host },
				{ "baseHost", _host }
			};
			if (!string.IsNullOrEmpty(_searchCommunityId))
				parameters.Add("communityId", _searchCommunityId);
			if (lang != null)
				parameters.Add("lang", lang);

			var message = new HttpRequestMessage(HttpMethod.Post, GetUri(_searchHost, _token, "/api/1.0/search", _scheme, parameters));

			if (!string.IsNullOrEmpty(_clientId) && !string.IsNullOrEmpty(_clientSecret))
				message.Headers.Add("Authorization", GetAuthorizationHeaderValue(_clientId, _clientSecret));

			var sendAsyncResp = await GetApiClient().SendAsync(message);
			sendAsyncResp.EnsureSuccessStatusCode();
			var strRead = await sendAsyncResp.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<SearchTimeSeriesResponse>(strRead);
		}

		private static string GetAuthorizationHeaderValue(string clientId, string clientSecret)
		{
			return string.Format("Knoema {0}:{1}:{2}", clientId,
					Convert.ToBase64String(new HMACSHA1(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("dd-MM-yy-HH", CultureInfo.InvariantCulture))).ComputeHash(Encoding.UTF8.GetBytes(clientSecret))),
					AuthProtoVersion);
		}

		private static Uri GetUri(string host, string token, string path, string scheme, Dictionary<string, string> parameters = null)
		{
			if (!string.IsNullOrEmpty(token))
			{
				if (parameters == null)
					parameters = new Dictionary<string, string>();
				parameters.Add("access_token", token);
			}
			var builder = new UriBuilder(scheme, host)
			{
				Path = path,
				Query = parameters != null ?
					string.Join("&", parameters.Select(pair => string.Format("{0}={1}", pair.Key, WebUtility.UrlEncode(pair.Value)))) :
					string.Empty
			};
			return builder.Uri;
		}

		public Task<T> GetTaskResult<T>(TaskResponse taskResponse) where T : TaskResult
		{
			if (taskResponse.ProxyData == null && taskResponse.TaskKey.HasValue)
				return ApiGet<T>("/api/1.0/meta/taskresult", string.Format("taskKey={0}", taskResponse.TaskKey.Value));

			return ApiPost<T>("/api/1.0/meta/taskresult", taskResponse);
		}

		public async Task<T> WaitTaskResult<T>(TaskResponse taskResponse, int spinDelayInSeconds, int maxWaitCount) where T : TaskResult
		{
			T taskResult = null;
			for (var i = 0; ;)
			{
				taskResult = await GetTaskResult<T>(taskResponse);
				if (!(taskResult.Status == Meta.TaskStatus.Executing || taskResult.Status == Meta.TaskStatus.Pending))
					break;
				i++;
				if (i >= maxWaitCount)
					throw new Exception("Maximum wait count reached");
				Thread.Sleep(spinDelayInSeconds * 1000);
			}

			if (taskResult.Status == Meta.TaskStatus.Cancelled)
				throw new TaskCanceledException("Task was cancelled");
			if (taskResult.Status == Meta.TaskStatus.Failed)
				throw new Exception(taskResult.Message);

			if (taskResult.Status == Meta.TaskStatus.Completed)
				return taskResult;

			throw new ArgumentOutOfRangeException("Unexpected task status");
		}

		private Task<TaskResponse> StartUnload(PivotRequest request)
		{
			return ApiPost<TaskResponse>("/api/1.0/data/unload", request);
		}

		private async Task<DatasetUnloadTaskResultData> Unload(PivotRequest request, int spinDelayInSeconds = 10, int maxWaitCount = 360)
		{
			var unloadTaskResponse = await StartUnload(request);
			var taskResult = await WaitTaskResult<DatasetUnloadTaskResult>(unloadTaskResponse, spinDelayInSeconds, maxWaitCount);
			return taskResult.Data;
		}

		private Task<Task>[] GetFilesAfterUnload(string[] files, Stream[] resultStreams)
		{
			var cts = new CancellationTokenSource();
			var clientHandler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
			};
			var client = new HttpClient(clientHandler) { Timeout = TimeSpan.FromMilliseconds(HttpTimeout) };
			var streamTasks = new Task<Task>[files.Length];

			try
			{
				var contentTasks = new Task<HttpResponseMessage>[files.Length];

				for (var i = 0; i < files.Length; i++)
				{
					contentTasks[i] = client.GetAsync(files[i], HttpCompletionOption.ResponseHeadersRead, cts.Token);
				}

				for (var i = 0; i < files.Length; i++)
				{
					var output = resultStreams[i];
					streamTasks[i] = contentTasks[i].ContinueWith(t =>
					{
						HttpResponseMessage response = null;
						HttpContent content = null;
						Stream dataStream = null;
						try
						{
							response = t.GetAwaiter().GetResult();
							content = response.Content;
							dataStream = content.ReadAsStreamAsync().GetAwaiter().GetResult();
							return dataStream.CopyToAsync(output, 4096 * 16, cts.Token).ContinueWith(_ =>
							{
								dataStream.Dispose();
								dataStream = null;
								content.Dispose();
								content = null;
								response.Dispose();
								response = null;
							});
						}
						catch (Exception)
						{
							cts.Cancel();
							if (dataStream != null)
								dataStream.Dispose();
							if (content != null)
								content.Dispose();
							if (response != null)
								response.Dispose();
							throw;
						}
					}, cts.Token);
				}
			}
			catch (Exception)
			{
				cts.Cancel();
				throw;
			}

			return streamTasks;
		}

		public async Task<string[]> UnloadToLocalFolder(PivotRequest request, string destinationFolder)
		{
			var unloadResult = await Unload(request);

			var files = unloadResult.Files.ToArray();
			var fileNames = new string[files.Length];
			var urls = new string[files.Length];
			var fileStreams = new Stream[files.Length];

			bool succeeded = false;
			try
			{
				for (var i = 0; i < files.Length; i++)
				{
					var file = files[i];
					fileStreams[i] = File.Create(Path.Combine(destinationFolder, file.Name));
					fileNames[i] = file.Name;
					urls[i] = file.Url;
				}

				var copyTasks = await Task.WhenAll(GetFilesAfterUnload(urls, fileStreams));
				await Task.WhenAll(copyTasks);
				succeeded = true;
			}
			finally
			{
				if (fileStreams != null)
				{
					for (var i = 0; i < fileStreams.Length; i++)
					{
						if (fileStreams[i] != null)
							fileStreams[i].Dispose();
					}
				}

				if (!succeeded && fileNames != null)
				{
					for (var i = 0; i < fileNames.Length; i++)
					{
						if (!string.IsNullOrEmpty(fileNames[i]))
						{
							try
							{
								File.Delete(destinationFolder + '\\' + fileNames[i]);
							}
							catch { }
						}
					}

					fileNames = null;
				}
			}

			return fileNames;
		}
	}
}
