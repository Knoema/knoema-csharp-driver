using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class ServerTimeseriesSearchResponse
	{
		public TimeseriesSearchFacets Facets { get; set; }
		public IEnumerable<TimeseriesSearchDatasetItem> Items { get; set; }
	}
}
