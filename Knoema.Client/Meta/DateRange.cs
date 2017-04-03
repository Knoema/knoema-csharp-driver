using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Meta
{
	public class DateRange
	{
		public object Calendar { get; set; }
		public DateTime EndDate { get; set; }
		public DateTime StartDate { get; set; }

		[JsonProperty(PropertyName = "frequencies")]
		public List<char> FrequenciesAbbr { get; set; }
	}
}
