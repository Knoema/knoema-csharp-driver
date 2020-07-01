using System;
using System.Collections.Generic;

namespace Knoema.Meta
{
	public class DatasetsStatisticsRequest
	{
		public DateTime? ExpiredDatesetsDateFrom { get; set; }
		public DateTime? ExpiredDatesetsDateTo { get; set; }
		public bool? ExpectingUpdate { get; set; }
		public IEnumerable<string> Ids { get; set; }
		public string Auto { get; set; }
		public IEnumerable<string> Statuses { get; set; }
		public bool? IncludePrivate { get; set; }
	}
}
