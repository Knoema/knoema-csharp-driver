using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class Facets : RequestFacets
	{
		public Dictionary<string, DatasetSourceItem> DataSourceInfos { get; set; }
		public Dictionary<string, GeographyRegion> RegionLinkInfos { get; set; }
	}
}