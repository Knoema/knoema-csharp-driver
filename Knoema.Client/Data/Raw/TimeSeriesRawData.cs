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
		public Dictionary<string, IEnumerable<string>> DetailValues { get; set; }


		internal void InitAfterDeserialized(string[] detailColumns)
		{
			var dimensions = new List<DimensionItem>();
			foreach (var pair in _extensionData)
			{
				var valueObj = pair.Value;
				if (valueObj.Type != JTokenType.Object || valueObj["key"] == null || valueObj["name"] == null)
					continue;
				var value = valueObj.ToObject<DimensionItem>();
				value.DimensionId = pair.Key;
				dimensions.Add(value);
			}

			Dimensions = dimensions;

			if (detailColumns != null)
			{
				DetailValues = new Dictionary<string, IEnumerable<string>>();
				foreach (var column in detailColumns)
				{
					_extensionData.TryGetValue(column, out var jsonData);
					DetailValues[column] = jsonData?.Values<string>().ToArray() ?? Enumerable.Empty<string>();
				}
			}

			_extensionData = null;
		}
	}
}
