using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class FlatResponseWithTokenData
	{
		[JsonExtensionData]
		public Dictionary<string, object> Dimensions { get; set; }
		public List<AttributesInFlatDataset> Data { get; set; }

		public List<FlatDatasetDimension> GetElement()
		{
			return Dimensions.Select(pair =>
			{
				var result = JsonConvert.DeserializeObject<FlatDatasetDimension>(pair.Value.ToString());
				result.DimensionId = pair.Key;
				return result;
			}).ToList();
		}
	}
}
