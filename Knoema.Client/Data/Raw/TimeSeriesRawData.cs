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
		private IDictionary<string, JToken> _extensionData;

		public IEnumerable<DimensionItem> Dimensions { get; set; }
		public Dictionary<string, string> TimeSeriesAttributes { get; set; }
		public Dictionary<string, IEnumerable<string>> DetailColumnsData { get; set; }


		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			var dimensions = new List<DimensionItem>();
			var columns = new Dictionary<string, IEnumerable<string>>();

			foreach (var pair in _extensionData)
			{
				var valueObj = pair.Value;
				if (valueObj.Type == JTokenType.Array)
				{
					columns.Add(pair.Key, valueObj.Values<string>().ToArray());
					continue;
				}

				if (valueObj.Type != JTokenType.Object || valueObj["key"] == null || valueObj["name"] == null)
					continue;

				var value = valueObj.ToObject<DimensionItem>();
				value.DimensionId = pair.Key;
				dimensions.Add(value);
			}

			Dimensions = dimensions;
			if (columns.Any())
				DetailColumnsData = columns;

			_extensionData = null;
		}
	}
}
