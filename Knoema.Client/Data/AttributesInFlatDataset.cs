using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class AttributesInFlatDataset
	{
		[JsonExtensionData]
		public Dictionary<string, object> Fields { get; set; }

		public List<AttributeFlat> GetDetails()
		{
			return Fields.Select(pair => ParseItem(pair)).ToList();
		}

		private AttributeFlat ParseItem(KeyValuePair<string, object> pair)
		{
			var str = pair.Value.ToString();
			if (str.Contains("date") && str.Contains("frequency"))
			{
				var result = JsonConvert.DeserializeObject<AttributeDate>(str);
				result.Name = pair.Key;
				result.Type = AttributeType.DateTime;
				return result;
			}
			else if (str.Contains("value") && str.Contains("unit"))
			{
				var result = JsonConvert.DeserializeObject<AttributeUnit>(str);
				result.Name = pair.Key;
				result.Type = AttributeType.Unit;
				return result;
			}
			else
				return new AttributeBase { Name = pair.Key, Value = pair.Value, Type = AttributeType.Base };
		}
	}
}
