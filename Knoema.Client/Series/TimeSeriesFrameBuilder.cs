using System;
using System.Collections.Generic;
using Deedle;
using Knoema.Data;
using Knoema.Meta;

namespace Knoema.Series
{
	public class TimeSeriesFrameBuilder
	{
		private readonly Dictionary<string, int>[] _dimensionKeyMaps;
		private readonly string[] _dimensions;
		private readonly IReadOnlyDictionary<string, int> _dimensionIdsMap;
		private readonly string[] _attributes;
		private readonly IReadOnlyDictionary<string, int> _attributeIds;
		private readonly Dictionary<TimeSeriesId, TimeSeriesValues> _values;
		private readonly Dictionary<Tuple<DateTime, DateTime, Frequency>, IReadOnlyList<DateTime>> _timeRangeCache;

		public TimeSeriesFrameBuilder(Dataset dataset, IReadOnlyDictionary<string, string> dimensionIdsMap, Func<string, Dictionary<string, int>> dimensionMappingFactory)
		{
			var dims = new List<string>();
			foreach (var dim in dataset.Dimensions)
				dims.Add(dim.Id);
			_dimensions = dims.ToArray();

			var dimensionIds = new Dictionary<string, int>();
			for (var i = 0; i < _dimensions.Length; i++)
				dimensionIds[_dimensions[i]] = i;
			if (dimensionIdsMap != null)
			{
				foreach (var p in dimensionIdsMap)
				{
					if (!dimensionIds.ContainsKey(p.Key))
					{
						int dimIndex;
						if (dimensionIds.TryGetValue(p.Value, out dimIndex))
							dimensionIds[p.Key] = dimIndex;
					}
				}
			}
			_dimensionIdsMap = dimensionIds;

			var attrs = new List<string>();
			attrs.AddRange(_dimensions);
			if (dataset.TimeSeriesAttributes != null)
			{
				foreach (var attr in dataset.TimeSeriesAttributes)
					attrs.Add(attr.Name);
			}
			_attributes = attrs.ToArray();

			var attributeIds = new Dictionary<string, int>(dimensionIds);
			for (var i = _dimensions.Length; i < _attributes.Length; i++)
				attributeIds[_attributes[i]] = i;
			_attributeIds = attributeIds;

			_dimensionKeyMaps = new Dictionary<string, int>[_dimensions.Length];
			for (var i = 0; i < _dimensionKeyMaps.Length; i++)
				_dimensionKeyMaps[i] = dimensionMappingFactory(_dimensions[i]) ?? new Dictionary<string, int>();

			_values = new Dictionary<TimeSeriesId, TimeSeriesValues>();
			_timeRangeCache = new Dictionary<Tuple<DateTime, DateTime, Frequency>, IReadOnlyList<DateTime>>();
		}

		public void AddRange(IEnumerable<RegularTimeSeriesRawData> seriesEnum)
		{
			foreach (var seriesItem in seriesEnum)
				Add(seriesItem);
		}

		public void Add(RegularTimeSeriesRawData item)
		{
			var dimensions = new int[_dimensions.Length];
			var attributes = new object[_attributes.Length];
			foreach (var dim in item.Dimensions)
			{
				var dimIndex = _dimensionIdsMap[dim.DimensionId];
				dimensions[dimIndex] = dim.Key;
				attributes[dimIndex] = dim.Name;
				_dimensionKeyMaps[dimIndex][dim.Name] = dim.Key;
			}

			if (item.TimeSeriesAttributes != null)
			{
				foreach (var attr in item.TimeSeriesAttributes)
				{
					var attrIndex = _attributeIds[attr.Key];
					attributes[attrIndex] = attr.Value;
				}
			}

			var frequency = item.Frequency.ToFrequency();
			var rangeTuple = Tuple.Create(item.StartDate, item.EndDate, frequency);
			IReadOnlyList<DateTime> timeRange;
			if (!_timeRangeCache.TryGetValue(rangeTuple, out timeRange))
			{
				timeRange = TimeFormat.InvariantTimeFormat.ExpandRangeSelection(item.StartDate, item.EndDate, frequency);
				_timeRangeCache[rangeTuple] = timeRange;
			}

			var valuesBuilder = new SeriesBuilder<DateTime, double>();
			int valueIndex = 0;
			foreach (var valueItem in item.Values)
			{
				if (valueItem != null)
					valuesBuilder.Add(timeRange[valueIndex], Convert.ToDouble(valueItem));
				valueIndex++;
			}

			var values = new TimeSeriesValues(
				item,
				frequency,
				new AttributesMap<int>(_dimensions, dimensions),
				new AttributesMap<object>(_attributes, attributes),
				valuesBuilder.Series);

			_values[values] = values;
		}

		public TimeSeriesFrame GetResult()
		{
			return new TimeSeriesFrame(_dimensionIdsMap, _dimensions, _dimensionKeyMaps, _attributes, _values);
		}
	}
}
