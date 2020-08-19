using System;
using System.Collections.Generic;
using System.Text;

namespace Knoema.Meta
{
	public enum AutoFilter
	{
		NoCheck,
		Auto,
		Manual
	}

	public class DataOpsDatasetsRequest
	{
		public DateTime? ExpiredDatesetsDateFrom { get; set; }

		public DateTime? ExpiredDatesetsDateTo { get; set; }

		public bool? ExpectingUpdate { get; set; }

		public IEnumerable<string> Ids { get; set; }

		public AutoFilter? Auto { get; set; }

		public IEnumerable<string> Statuses { get; set; }

		public bool? IncludePrivate { get; set; }
	}
}
