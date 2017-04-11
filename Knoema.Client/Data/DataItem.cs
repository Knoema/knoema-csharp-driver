using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Knoema.Data
{
	public class DataItem
	{
		[JsonExtensionData]
		private Dictionary<string, JToken> _fields;

		public List<DataItemValue> Values { get; set; }

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			Values = _fields.Select(ParseItem).ToList();
		}

		private DataItemValue ParseItem(KeyValuePair<string, JToken> pair)
		{
			if (pair.Value.Type != JTokenType.Object)
				return new DataItemDetail { Type = DataItemType.Detail, Name = pair.Key, Value = pair.Value };

			var result = pair.Value.ToObject<DataItemValue>();
			result.Name = pair.Key;
			return result;
		}
	}
}
