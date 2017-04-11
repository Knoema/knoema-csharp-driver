using System;
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
		private Dictionary<string, JToken> _dimensions;

		public List<DimensionItem> Dimensions { get; set; }

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			Dimensions = _dimensions.Select(ParseItem).ToList();
		}

		private DimensionItem ParseItem(KeyValuePair<string, JToken> pair)
		{
			var result = pair.Value.ToObject<DimensionItem>();
			result.DimensionId = pair.Key;
			return result;
		}
	}
}
