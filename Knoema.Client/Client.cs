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

namespace Knoema
{
    public class Client
    {
		string _appId;
		string _appSecret;
		Uri _uri;

		public Client(string host)
		{
			_uri = new UriBuilder(Uri.UriSchemeHttp, host).Uri;
		}

		public Client(string host, string appId, string appSecret)
		{
			_uri = new UriBuilder(Uri.UriSchemeHttp, host).Uri;

			_appId = appId;
			_appSecret = appSecret;
		}

		public Task<Dataset> GetDataset(string id)
		{
			return JsonConvert.DeserializeObjectAsync<Dataset>(
				DataRequest(string.Format("/api/1.0/meta/dataset/{0}", id)).Result
			);
		}

		public Task<Dimension> GetDatasetDimension(string dataset, string dimension)
		{
			return JsonConvert.DeserializeObjectAsync<Dimension>(
				DataRequest(string.Format("/api/1.0/meta/dataset/{0}/dimension/{1}", dataset, dimension)).Result
			);
		}

		public Task<PivotResponse> GetData(PivotRequest pivot)
		{
			return JsonConvert.DeserializeObjectAsync<PivotResponse>(
				DataRequest("/api/1.0/data/pivot/", JsonConvert.SerializeObject(pivot)).Result
			);
		}

		Task<string> DataRequest(string path, string data = null)
		{
			var builder = new UriBuilder(_uri);
			builder.Path = path;

			
			var client = new HttpClient();

			if (!string.IsNullOrEmpty(_appId) && !string.IsNullOrEmpty(_appSecret))
				client.DefaultRequestHeaders.Add("Authorization",
					string.Format("Knoema {0}:{1}", _appId, 
						Convert.ToBase64String(
							new HMACSHA1(
								Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("dd-mm-yy-hh"))).ComputeHash(Encoding.UTF8.GetBytes(_appSecret)))
					)
				);

			if (string.IsNullOrEmpty(data))
				return client.GetStringAsync(builder.Uri);
			else
			{
				var content = new StringContent(data);
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				return client.PostAsync(builder.Uri, content).Result.Content.ReadAsStringAsync();
			}
		}
    }
}
