using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Knoema.Search;

namespace Knoema
{
	public class SearchClient : ClientBase
	{
		private const string _apiSearch = @"/api/1.0/search";

		public SearchClient(string host)
			: base(host)
		{ }

		public SearchClient(string host, string token)
			: base(host, token)
		{ }

		public SearchClient(string host, string appId, string appSecret)
			: base(host, appId, appSecret)
		{ }

		public Task<SearchTimeSeriesResponse> Search(string searchText, SearchScope scope, int count, int version)
		{
			var parameters = HttpUtility.ParseQueryString("");
			parameters.Add("query", searchText.Trim());
			parameters.Add("scope", SearchScopeUtil.GetString(scope));
			parameters.Add("count", count.ToString());
			parameters.Add("version", version.ToString());
			return _accessor.Get<SearchTimeSeriesResponse>(_apiSearch, parameters);
		}
	}
}
