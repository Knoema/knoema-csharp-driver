using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private readonly string _host;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _token;

        private string _searchHost;
        private string _searchCommunityId;

        private const string AuthProtoVersion = "1.2";
        private const int HttpClientTimeout = 300 * 1000;

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

        public Client(string host, string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host");

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException("clientId");

            if (string.IsNullOrEmpty(clientSecret))
                throw new ArgumentNullException("clientSecret");

            _host = host;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        private HttpClient GetApiClient()
        {
            var clientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            var client = new HttpClient(clientHandler) { Timeout = TimeSpan.FromMilliseconds(HttpClientTimeout) };

            if (!string.IsNullOrEmpty(_clientId) && !string.IsNullOrEmpty(_clientSecret))
                client.DefaultRequestHeaders.Add("Authorization",
                    string.Format("Knoema {0}:{1}:{2}", _clientId,
                        Convert.ToBase64String(
                            new HMACSHA1(
                                Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("dd-MM-yy-HH"))).ComputeHash(Encoding.UTF8.GetBytes(_clientSecret))),
                                AuthProtoVersion
                    )
                );

            return client;
        }

        private async Task<T> ApiGet<T>(string path, string query = null)
        {
            var builder = new UriBuilder(Uri.UriSchemeHttp, _host);

            if (!string.IsNullOrEmpty(path))
                builder.Path = path;

            if (!string.IsNullOrEmpty(_token))
                query = query + "&access_token=" + _token;

            if (!string.IsNullOrEmpty(query))
                builder.Query = query;

            var response = await GetApiClient().GetStringAsync(builder.Uri);
            return JsonConvert.DeserializeObject<T>(response);
        }

        private async Task<T> ApiPost<T>(string path, HttpContent content)
        {
            var builder = new UriBuilder(Uri.UriSchemeHttp, _host);
            builder.Path = path;

            if (!string.IsNullOrEmpty(_token))
                builder.Query = "access_token=" + _token;

            var postResponse = await GetApiClient().PostAsync(builder.Uri, content);
            var readString = await postResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(readString);
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

        public Task<VerifyDatasetResult> VerifyDataset(string id, DateTime? publicationDate = null, string source = null, string refUrl = null)
        {
            return ApiPost<VerifyDatasetResult>("/api/1.0/meta/verifydataset", new
            {
                id = id,
                publicationDate = publicationDate,
                source = source,
                refUrl = refUrl
            });
        }

        public Task<DateRange> GetDatasetDateRange(string datasetId)
        {
            return ApiGet<DateRange>(string.Format("/api/1.0/meta/dataset/{0}/daterange", datasetId));
        }

        public async Task<SearchTimeSeriesResponse> Search(string searchText, SearchScope scope, int count, int version, string lang = null)
        {
            if (_searchHost == null)
            {
                var configResponse = await ApiGet<ConfigResponse>("/api/1.0/search/config");
                _searchHost = configResponse.SearchHost;
                _searchCommunityId = configResponse.CommunityId;
            }

            var parameters = new Dictionary<string, string>();
            parameters.Add("query", searchText.Trim());
            parameters.Add("scope", scope.GetString());
            if (!string.IsNullOrEmpty(_searchCommunityId))
                parameters.Add("communityId", _searchCommunityId);
            parameters.Add("count", count.ToString());
            parameters.Add("version", version.ToString());
            parameters.Add("host", _host);
            if (lang != null)
                parameters.Add("lang", lang);

            var message = new HttpRequestMessage(HttpMethod.Post, GetUri(_searchHost, _token, "/api/1.0/search", parameters));

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
                    Convert.ToBase64String(new HMACSHA1(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("dd-MM-yy-HH"))).ComputeHash(Encoding.UTF8.GetBytes(clientSecret))),
                    AuthProtoVersion);
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
    }
}
