using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class TimeseriesSearchRequestFacets
	{
		public Dictionary<string, int> DataSources { get; set; }
		public int Forecasts { get; set; }
		public Dictionary<string, int> Frequencies { get; set; }
		public Dictionary<string, int> LastUpdates { get; set; }
		public int LongTimeseries { get; set; }
		public Dictionary<string, int> RegionLinks { get; set; }
	}
}
