using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Knoema.Data
{
	public abstract class TimeSeriesRawData
	{
		[JsonExtensionData]
		private IDictionary<string, JToken> _dimensions;

		public IEnumerable<DimensionItem> Dimensions { get; set; }
		public Dictionary<string, string> TimeSeriesAttributes { get; set; }

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			Dimensions = _dimensions.Select(pair =>
			{
				var value = pair.Value.ToObject<DimensionItem>();
				value.DimensionId = pair.Key;
				return value;
			});
			_dimensions = null;
		}
	}
}
