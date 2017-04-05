using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public abstract class TimeSeriesRawData
	{
		[JsonExtensionData]
		public Dictionary<string, object> Dimensions { get; set; }

		public IEnumerable<DimensionItem> GetDimensions()
		{
			return Dimensions.Select(pair =>
			{
				var result = JsonConvert.DeserializeObject<DimensionItem>(pair.Value.ToString());
				result.DimensionId = pair.Key;
				return result;
			});
		}
	}
}
