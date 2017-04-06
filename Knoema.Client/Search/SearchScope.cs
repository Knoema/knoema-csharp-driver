using System;
using System.Collections.Generic;
using System.Linq;

namespace Knoema.Search
{
	[Flags]
	public enum SearchScope
	{
		Timeseries = 0x8,
		NamedEntity = 0x100,
		Atlas = 0x200,
		Semantic = 0x800
	}

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
