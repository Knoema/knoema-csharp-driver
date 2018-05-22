using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Knoema
{
	public abstract class JsonCreationConverter<T> : JsonConverter
	{
		/// <summary>Determines if this converted is designed to deserialization to objects of the specified type.</summary>
		/// <param name="objectType">The target type for deserialization.</param>
		/// <returns>True if the type is supported.</returns>
		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		/// <summary>Parses the json to the specified type.</summary>
		/// <param name="reader">Newtonsoft.Json.JsonReader</param>
		/// <param name="objectType">Target type.</param>
		/// <param name="existingValue">Ignored</param>
		/// <param name="serializer">Newtonsoft.Json.JsonSerializer to use.</param>
		/// <returns>Deserialized Object</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;

			// Load JObject from stream
			var jObject = JObject.Load(reader);

			// Create target object based on JObject
			var target = Create(objectType, jObject);

			//Create a new reader for this jObject, and set all properties to match the original reader.
			var jObjectReader = jObject.CreateReader();
			jObjectReader.Culture = reader.Culture;
			jObjectReader.DateParseHandling = reader.DateParseHandling;
			jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
			jObjectReader.FloatParseHandling = reader.FloatParseHandling;

			// Populate the object properties
			serializer.Populate(jObjectReader, target);

			return target;
		}

		/// <summary>Serializes to the specified type</summary>
		/// <param name="writer">Newtonsoft.Json.JsonWriter</param>
		/// <param name="value">Object to serialize.</param>
		/// <param name="serializer">Newtonsoft.Json.JsonSerializer to use.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		protected abstract T Create(Type objectType, JObject jObject);
	}
}
