using System;
using System.Collections;
using System.Collections.Generic;

namespace Knoema.Series
{
	public struct AttributesMap<T> : IReadOnlyDictionary<string, T>
	{
		private readonly string[] _keys;
		private readonly T[] _values;

		public AttributesMap(string[] keys, T[] values)
		{
			_keys = keys;
			_values = values;
		}

		public string[] Keys
		{
			get
			{
				return _keys;
			}
		}

		public T[] Values
		{
			get
			{
				return _values;
			}
		}

		private int IndexOfKey(string key)
		{
			return Array.IndexOf(_keys, key);
		}

		public bool ContainsKey(string key)
		{
			return IndexOfKey(key) >= 0;
		}

		IEnumerable<string> IReadOnlyDictionary<string, T>.Keys
		{
			get { return _keys; }
		}

		public bool TryGetValue(string key, out T value)
		{
			var index = IndexOfKey(key);
			if (index >= 0)
			{
				value = _values[index];
				return true;
			}

			value = default(T);
			return false;
		}

		IEnumerable<T> IReadOnlyDictionary<string, T>.Values
		{
			get { return _values; }
		}

		public T this[string key]
		{
			get
			{
				var index = IndexOfKey(key);
				if (index >= 0)
					return _values[index];
				throw new KeyNotFoundException();
			}
		}

		public int Count
		{
			get { return _keys.Length; }
		}

		public IEnumerator<KeyValuePair<string, T>> GetEnum()
		{
			for (int i = 0; i < _keys.Length; i++)
				yield return new KeyValuePair<string, T>(_keys[i], _values[i]);
		}

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			return GetEnum();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnum();
		}
	}
}
