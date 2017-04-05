using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class DataItem
	{
		[JsonExtensionData]
		public Dictionary<string, object> Fields { get; set; }

		public IEnumerable<DataItemValue> GetDetails()
		{
			return Fields.Select(pair => ParseItem(pair));
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
