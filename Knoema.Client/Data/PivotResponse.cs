using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class PivotResponse
	{
		public List<PivotDimensionItem> Header { get; set; }
		public List<PivotDimensionItem> Stub { get; set; }
		public List<PivotDimensionItem> Filter { get; set; }

		[JsonProperty(PropertyName = "data")]
		public PivotDataTuples Tuples { get; private set; }
	}
}
