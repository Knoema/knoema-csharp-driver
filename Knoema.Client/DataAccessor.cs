using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Knoema
{
	public class DataAccessor
	{
		private readonly string _host;
		private readonly string _appId;
		private readonly string _appSecret;
		private readonly string _token;
		private readonly HttpClient _httpClient;

		private const string _authProtoVersion = "1.2";
		private const int _httpClientTimeout = 300 * 1000;

		public DataAccessor(string host)
		{
			_host = host;
			var clientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
			_httpClient = new HttpClient(clientHandler) { Timeout = TimeSpan.FromMilliseconds(_httpClientTimeout) };
		}

		public DataAccessor(string host, string token)
			: this(host)
		{
			_token = token;
		}

		public DataAccessor(string host, string appId, string appSecret)
			: this(host)
		{
			_appId = appId;
			_appSecret = appSecret;
		}

		public Uri GetUri(string path, NameValueCollection parameters = null)
		{
			if (!string.IsNullOrEmpty(_token))
			{
				if (parameters == null)
					parameters = HttpUtility.ParseQueryString(string.Empty);
				parameters.Add("access_token", _token);
			}
			var builder = new UriBuilder
			{
				Host = _host,
				Path = path,
				Query = parameters != null ? parameters.ToString() : string.Empty
			};
			return builder.Uri;
		}

		public Task<T> Get<T>(string path, NameValueCollection parameters = null)
		{
			var message = new HttpRequestMessage(HttpMethod.Get, GetUri(path, parameters));
			return Access<T>(message);
		}

		public Task<TResponse> Post<TRequest, TResponse>(string path, TRequest request, NameValueCollection parameters = null)
		{
			var message = new HttpRequestMessage(HttpMethod.Post, GetUri(path, parameters))
			{
				Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
			};
			return Access<TResponse>(message);
		}

		public Task<TResponse> Access<TResponse>(HttpRequestMessage message)
		{
			if (!string.IsNullOrEmpty(_appId) && !string.IsNullOrEmpty(_appSecret))
				message.Headers.Add("Authorization", GetAuthorizationValue(_appId, _appSecret));

			return _httpClient.SendAsync(message).
				Then(resp => resp.Content.ReadAsStringAsync()).
				Then(resp => Task.Run(() => JsonConvert.DeserializeObject<TResponse>(resp)));
		}

		private static string GetAuthorizationValue(string appId, string appSecret)
		{
			return string.Format("Knoema {0}:{1}:{2}", appId,
					Convert.ToBase64String(new HMACSHA1(Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("dd-MM-yy-HH"))).ComputeHash(Encoding.UTF8.GetBytes(appSecret))),
					_authProtoVersion);
		}
	}
}
