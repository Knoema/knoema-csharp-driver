using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class PivotDatasetDimension
	{
		public string DimensionId { get; set; }
		public int Key { get; set; }
		public string Name { get; set; }

		[JsonExtensionData]
		public Dictionary<string, object> Fields { get; set; }
	}
}
