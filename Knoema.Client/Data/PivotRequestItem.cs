using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class PivotRequestItem
	{
		public string DimensionId { get; set; }
		public List<object> Members { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string UiMode { get; set; }

		public PivotRequestItem()
		{
			Members = new List<object>();
		}
	}
}
