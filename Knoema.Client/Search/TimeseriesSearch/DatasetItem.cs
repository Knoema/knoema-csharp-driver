using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class DatasetItem
	{
		public DatasetDescriptor Dataset { get; set; }
		public Facets Facets { get; set; }
		public IEnumerable<TimeSeriesDescriptor> Items { get; set; }
		public object Location { get; set; }
	}
}