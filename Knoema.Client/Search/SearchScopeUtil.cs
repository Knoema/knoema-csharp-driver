using System;
using System.Collections.Generic;
using System.Linq;

namespace Knoema.Search
{
	static class SearchScopeUtil
	{
		private readonly static List<SearchScope> _allScopes = new List<SearchScope>(Enum.GetValues(typeof(SearchScope)).Cast<SearchScope>());

		private readonly static Dictionary<SearchScope, string> _scopeToString = new Dictionary<SearchScope, string>
		{
			{ SearchScope.Timeseries, "timeseries" },
			{ SearchScope.Atlas, "atlas" },
			{ SearchScope.NamedEntity, "namedentity" },
			{ SearchScope.Semantic, "semantic" },
		};

		public static string GetString(this SearchScope scope)
		{
			var singleScopeList = new List<SearchScope>();

			foreach (var item in _allScopes)
				if ((scope & item) == item)
					singleScopeList.Add(item);

			return string.Join(",", singleScopeList.Select(s => _scopeToString[s]));
		}
	}
}
