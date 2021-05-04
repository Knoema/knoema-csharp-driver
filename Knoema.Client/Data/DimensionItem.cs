using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class DimensionItem
	{
		public string DimensionId { get; set; }
		public string Key { get; set; }
		public string Name { get; set; }
		[JsonExtensionData]
		public IDictionary<string, object> MetadataFields { get; set; }
	}
}
