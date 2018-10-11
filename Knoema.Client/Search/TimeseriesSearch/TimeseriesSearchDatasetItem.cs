using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class TimeseriesSearchDatasetItem
	{
		public DatasetDescriptor Dataset { get; set; }
		public TimeseriesSearchFacets Facets { get; set; }
		public IEnumerable<TimeSeriesDescriptor> Items { get; set; }
		public object Location { get; set; }
	}
}