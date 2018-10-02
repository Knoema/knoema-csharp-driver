using System.Collections.Generic;

namespace Knoema.Search
{
	public class TimeSeriesDescriptor
	{
		public DatasetDescriptor Dataset { get; set; }
		public IEnumerable<DimensionMemberDescriptor> Dimensions { get; set; }
		public IEnumerable<RegionLink> RegionLinks { get; set; }
		public TimeseriesSearch.TimeseriesSearchLocation Location { get; set; }
		public int TimeSeriesKey { get; set; }
		public float Weight { get; set; }
		public double UsedPart { get; set; }
		public string Owner { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public bool HasForecasting { get; set; }
		public bool IsLongTimeSeries { get; set; }
		public bool IsLowWeight { get; set; }
		public string Title { get; set; }
		public string Type { get; set; }
		public string Frequency { get; set; }
	}
}
