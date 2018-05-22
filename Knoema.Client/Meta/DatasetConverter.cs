using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Knoema.Meta
{
	class DatasetConverter : JsonCreationConverter<Dataset>
	{
		protected override Dataset Create(Type objectType, JObject jObject)
		{
			var settingsToken = jObject["settings"];
			if (settingsToken == null)
			{
				return new Dataset();
			}
			else
			{
				var settingsString = settingsToken.Value<string>();
				if (string.IsNullOrEmpty(settingsString))
					return new Dataset();
				else
					return new Dataset { Settings = JsonConvert.DeserializeObject<DatasetSettings>(settingsString) };
			}
		}
	}
}
