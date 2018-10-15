using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class Response
	{
		public Facets Facets { get; set; }
		public IEnumerable<DatasetItem> Items { get; set; }
	}
}
