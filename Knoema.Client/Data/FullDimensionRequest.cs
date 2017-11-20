using System;
using System.Collections.Generic;

namespace Knoema.Data
{
	public class FullDimensionRequest
	{
		public List<DimensionRequestItem> DimensionRequest { get; set; }
		public int Calendar { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public int? DateColumn { get; set; }
		public List<string> Frequency { get; set; }
		public IEnumerable<TimeseriesAttributeModel> TimeseriesAttributes { get; set; }
	}

	public class DimensionRequestItem
	{
		public string DimensionId { get; set; }
		public List<int> Members { get; set; }
	}

	public class TimeseriesAttributeModel
	{
		public string Name { get; set; }
		public string[] Values { get; set; }
	}
}
