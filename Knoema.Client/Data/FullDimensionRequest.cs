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

		public FullDimensionRequest()
		{
			DimensionRequest = new List<DimensionRequestItem>();
		}
	}
}
