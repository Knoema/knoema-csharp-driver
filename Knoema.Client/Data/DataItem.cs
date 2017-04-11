using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class DataItem
	{
		[JsonExtensionData]
		private Dictionary<string, DataItemValue> _fields;

		public List<DataItemValue> Values { get; set; }

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			Values = _fields.Select(pair => 
			{
				var value = pair.Value;
				value.Name = pair.Key;
				return value;
			}).ToList();
		}
	}
}
