using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public abstract class TimeSeriesRawData
	{
		[JsonExtensionData]
		private Dictionary<string, object> _dimensions;

		public IList<DimensionItem> Dimensions { get; set; }

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			Dimensions = _dimensions.Select(pair =>
			{
				var result = JsonConvert.DeserializeObject<DimensionItem>(pair.Value.ToString());
				result.DimensionId = pair.Key;
				return result;
			}).ToList();
		}
	}
}
