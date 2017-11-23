using System;
using System.Collections.Generic;

namespace Knoema.Data
{
	public class TimeSeriesItem
	{
		public string DatasetId { get; set; }
		public string Frequency { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int TimeseriesKey { get; set; }
		public List<DimensionItemForTimeSeriesItem> Metadata { get; set; }

		public TimeSeriesItem()
		{
			Metadata = new List<DimensionItemForTimeSeriesItem>();
		}
	}

	public class DimensionItemForTimeSeriesItem
	{
		public string Dim { get; set; }
		public int Key { get; set; }
		public string Name { get; set; }
	}
}
