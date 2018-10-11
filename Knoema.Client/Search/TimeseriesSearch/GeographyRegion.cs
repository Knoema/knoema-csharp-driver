using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class GeographyRegion
	{
		public string Id { get; set; }
		public string LocalizedName { get; set; }
		public string Name { get; set; }
		public IEnumerable<string> Parents { get; set; }
	}
}
