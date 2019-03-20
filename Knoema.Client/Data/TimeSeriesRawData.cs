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
				if (pair.Value.Type != JTokenType.Object || pair.Value["key"] == null || pair.Value["name"] == null)
					return null;
				var value = pair.Value.ToObject<DimensionItem>();
				value.DimensionId = pair.Key;
				return value;
			}).Where(d => d != null);
			_dimensions = null;
		}
	}
}
