using System.Collections.Generic;

namespace Knoema.Search
{
	public class SearchTimeSeriesResponse
	{
		public IEnumerable<TimeSeriesDescriptor> Items { get; set; }
		public IEnumerable<string> MainQueryRegionIds { get; set; }
		public IEnumerable<RegionLink> MainQueryRegionsList { get; set; }
		public string QueryRegions { get; set; }
		public IEnumerable<RegionLink> QueryRegionsList { get; set; }
	}
}
