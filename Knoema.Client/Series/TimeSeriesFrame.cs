using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deedle;

namespace Knoema.Series
{
	public class TimeSeriesFrame : IReadOnlyDictionary<TimeSeriesId, TimeSeriesValues>
	{
		private readonly IReadOnlyDictionary<string, int> _dimensionIdsMap;
		private readonly string[] _dimensions;
		private readonly string[] _attributes;
		private readonly IReadOnlyDictionary<string, int>[] _dimensionKeyMaps;
		private readonly IReadOnlyDictionary<TimeSeriesId, TimeSeriesValues> _values;

		public TimeSeriesFrame(IReadOnlyDictionary<string, int> dimensionIdsMap, string[] dimensions, IReadOnlyDictionary<string, int>[] dimensionKeyMaps, string[] attributes, IReadOnlyDictionary<TimeSeriesId, TimeSeriesValues> values)
		{
			_dimensionIdsMap = dimensionIdsMap;
			_dimensions = dimensions;
			_attributes = attributes;
			_dimensionKeyMaps = dimensionKeyMaps;
			_values = values;
		}

		public IReadOnlyList<string> Dimensions
		{
			get
			{
				return _dimensions;
			}
		}

		public Frame<TimeSeriesId, DateTime> ToFrame()
		{
			return Frame.FromRows(_values.Values.Select(v => new KeyValuePair<TimeSeriesId, Series<DateTime, double>>(v, v.Values)));
		}

		public Frame<string, DateTime> ToMnemonicsFrame()
		{
			return Frame.FromRows(_values.Values.Select(v => new KeyValuePair<string, Series<DateTime, double>>(v.Mnemonics, v.Values)));
		}

		public bool ContainsKey(TimeSeriesId key)
		{
			return _values.ContainsKey(key);
		}

		public IEnumerable<TimeSeriesId> Keys
		{
			get { return _values.Keys; }
		}

		public bool TryGetValue(TimeSeriesId key, out TimeSeriesValues value)
		{
			return _values.TryGetValue(key, out value);
		}

		public IEnumerable<TimeSeriesValues> Values
		{
			get { return _values.Values; }
		}

		public TimeSeriesValues this[TimeSeriesId key]
		{
			get
			{
				TimeSeriesValues res;
				_values.TryGetValue(key, out res);
				return res;
			}
		}

		public TimeSeriesId MakeId(string frequency, int[] keys, object[] attributes = null)
		{
			return MakeId(frequency.ToFrequency(), keys, attributes);
		}

		public TimeSeriesId MakeId(Frequency frequency, int[] keys, object[] attributes = null)
		{
			if (attributes == null)
			{
				attributes = new object[_attributes.Length];
				for (var i = 0; i < keys.Length; i++)
					attributes[i] = keys[i];
			}

			return new TimeSeriesId(frequency,
				new AttributesMap<int>(_dimensions, keys),
				new AttributesMap<object>(_attributes, attributes));
		}

		public TimeSeriesValues this[object key]
		{
			get
			{
				var id = MakeId(key.ToPropertySet());
				if (id == null)
					return default(TimeSeriesValues);

				return this[id];
			}
		}

		public TimeSeriesId MakeId(IEnumerable<KeyValuePair<string, object>> properties)
		{
			var frequency = Frequency.Annual;

			var dims = new int[_dimensions.Length];
			var attrs = new string[_attributes.Length];
			foreach (var pair in properties)
			{
				var name = pair.Key;
				var value = pair.Value;
				if (string.Equals(name, "Frequency", StringComparison.OrdinalIgnoreCase))
				{
					var val = Convert.ToString(value);
					frequency = val.ToFrequency();
				}
				else
				{
					int dimIndex;
					if (!_dimensionIdsMap.TryGetValue(name, out dimIndex))
						dimIndex = -1;
					var attrIndex = dimIndex >= 0 ? dimIndex : Array.IndexOf(_attributes, name, _dimensions.Length);
					if (dimIndex >= 0)
					{
						if (value is int)
						{
							int dimKey = (int)value;
							dims[dimIndex] = dimKey;
							if (attrIndex >= 0)
								attrs[attrIndex] = dimKey.ToString();
						}
						else
						{
							var val = Convert.ToString(value);
							int dimKey;
							if (_dimensionKeyMaps[dimIndex].TryGetValue(val, out dimKey))
								dims[dimIndex] = dimKey;
							attrs[attrIndex] = val;
						}
					}
					else
					{
						if (attrIndex >= 0)
							attrs[attrIndex] = Convert.ToString(value);
					}
				}
			}

			return MakeId(frequency, dims, attrs);
		}

		public int Count
		{
			get { return _values.Count; }
		}

		private IEnumerator<KeyValuePair<TimeSeriesId, TimeSeriesValues>> GetEnum()
		{
			return _values.GetEnumerator();
		}

		public IEnumerator<KeyValuePair<TimeSeriesId, TimeSeriesValues>> GetEnumerator()
		{
			return GetEnum();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnum();
		}
	}
}
