using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public abstract class PivotResponseBase<T>
	{
		public List<T> Filter { get; set; }
		public List<T> Header { get; set; }
		public List<T> Stub { get; set; }
		[JsonProperty(PropertyName = "data")]
		public PivotDataTuples Tuples { get; set; }
	}
}
