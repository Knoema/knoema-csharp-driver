using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Knoema
{
	public abstract class FieldCollection<T>
	{
		[JsonExtensionData]
		private IDictionary<string, JToken> _fields;

		public IEnumerable<T> Items { get; set; }

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			if (_fields != null)
			{
				var items = new List<T>();
				foreach (var pair in _fields)
				{
					var item = pair.Value.ToObject<T>();
					AttachKeyToObject(pair.Key, item);
					items.Add(item);
				}
				Items = items;
				_fields = null;
			}
		}

		protected abstract void AttachKeyToObject(string key, T obj);
	}
}
