using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class PivotResponse
	{
		public IList<PivotDimensionItem> Header { get; set; }
		public IList<PivotDimensionItem> Stub { get; set; }
		public IList<PivotDimensionItem> Filter { get; set; }

		[JsonProperty(PropertyName = "data")]
		public PivotDataTuples Tuples { get; private set; }
	}
}
