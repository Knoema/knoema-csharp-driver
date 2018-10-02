using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class TimeseriesSearchRequest
	{
		public int Count { get; set; }
		public TimeseriesSearchRequestFacets FacetsFilter { get; set; }
		public List<TimeseriesSearchLocation> Locations { get; set; }
		public bool PrepareFacets { get; set; }
	}
}
