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
		private IDictionary<string, JToken> _fields;

		public IEnumerable<DataItemValue> Values { get; set; }

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			Values = _fields.Select(pair =>
			{
				if (pair.Value.Type != JTokenType.Object)
					return new DataItemDetail { Type = DataItemType.Detail, Name = pair.Key, Value = pair.Value == null ? null : pair.Value.ToObject<string>() };

				var value = pair.Value.ToObject<DataItemValue>();
				value.Name = pair.Key;
				return value;
			});
			_fields = null;
		}
	}
}
