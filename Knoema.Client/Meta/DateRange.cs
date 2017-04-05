using System;
using System.Collections.Generic;

namespace Knoema.Meta
{
	public class DateRange
	{
		public object Calendar { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public IList<char> Frequencies { get; set; }

		public DateRange()
		{
			Frequencies = new List<char>();
		}
	}
}
