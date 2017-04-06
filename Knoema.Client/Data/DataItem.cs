using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class DataItem
	{
		[JsonExtensionData]
		private Dictionary<string, object> _fields;

		public IList<DataItemValue> Values { get; set; }

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			Values = _fields.Select(ParseItem).ToList();
		}

		private DataItemValue ParseItem(KeyValuePair<string, object> pair)
		{
			var str = pair.Value.ToString();
			if (str.Contains("date") && str.Contains("frequency"))
			{
				var result = JsonConvert.DeserializeObject<DataItemTime>(str);
				result.Name = pair.Key;
				result.Type = DataItemType.Time;
				return result;
			}
			else if (str.Contains("value") && str.Contains("unit"))
			{
				var result = JsonConvert.DeserializeObject<DataItemMeasure>(str);
				result.Name = pair.Key;
				result.Type = DataItemType.Measure;
				return result;
			}
			else
				return new DataItemDetail { Name = pair.Key, Value = pair.Value, Type = DataItemType.Detail };
		}
	}
}
