using System;
using System.Collections.Generic;

namespace Knoema.Data
{
	public class RegularTimeSeriesRawData : TimeSeriesRawData
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Frequency { get; set; }
		public string Unit { get; set; }
		public float Scale { get; set; }
		public string Mnemonics { get; set; }
		public IEnumerable<object> Values { get; set; }
	}
}
