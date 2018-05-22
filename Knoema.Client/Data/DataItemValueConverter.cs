using Newtonsoft.Json.Linq;
using System;

namespace Knoema.Data
{
	class DataItemValueConverter : JsonCreationConverter<DataItemValue>
	{
		protected override DataItemValue Create(Type objectType, JObject jObject)
		{
			if (FieldExists("date", jObject) && FieldExists("frequency", jObject))
				return new DataItemTime { Type = DataItemType.Time };
			else if (FieldExists("value", jObject) && FieldExists("unit", jObject))
				return new DataItemMeasure { Type = DataItemType.Measure };
			throw new Exception("Unknown type of data item.");
		}

		private bool FieldExists(string fieldName, JObject jObject)
		{
			return jObject[fieldName] != null;
		}
	}
}
