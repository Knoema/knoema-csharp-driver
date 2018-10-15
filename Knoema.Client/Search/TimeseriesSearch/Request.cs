using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class Request
	{
		public int Count { get; set; }
		public RequestFacets FacetsFilter { get; set; }
		public List<object> Locations { get; set; }
		public bool PrepareFacets { get; set; }
	}
}
