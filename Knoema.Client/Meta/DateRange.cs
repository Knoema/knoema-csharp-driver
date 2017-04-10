using System;
using System.Collections.Generic;

namespace Knoema.Meta
{
	public class DateRange
	{
		public int Calendar { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public List<string> Frequencies { get; set; }

		public DateRange()
		{
			Frequencies = new List<string>();
		}
	}
}
