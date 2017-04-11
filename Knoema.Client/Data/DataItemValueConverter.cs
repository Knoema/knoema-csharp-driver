using System;
using Newtonsoft.Json.Linq;

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
			return new DataItemDetail { Type = DataItemType.Detail };
		}

		private bool FieldExists(string fieldName, JObject jObject)
		{
			return jObject[fieldName] != null;
		}
	}
}
