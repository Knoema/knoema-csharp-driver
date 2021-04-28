using System;
using System.Collections.Generic;
using System.Linq;
using Knoema.Data;
using Knoema.Meta;

namespace Knoema.Series
{
	public class TimeSeriesRequestBuilder
	{
		private readonly Client _client;
		private IEnumerable<string> _frequencies;
		private readonly IReadOnlyDictionary<string, string> _dimensionIdsMap;
		private readonly Dictionary<string, Dictionary<string, string>> _dimensions;
		private readonly Dataset _dataset;

		public TimeSeriesRequestBuilder(Client client, Dataset dataset, IEnumerable<KeyValuePair<string, object>> request)
		{
			_client = client;
			_dataset = dataset;
			_dimensions = new Dictionary<string, Dictionary<string, string>>();

			var dimensionIdsMap = new Dictionary<string, string>();
			if (dataset.Dimensions != null)
			{
				foreach (var dim in dataset.Dimensions)
					dimensionIdsMap[dim.Name.Replace(' ', '_').Replace('-', '_')] = dim.Id;
				foreach (var dim in dataset.Dimensions)
					dimensionIdsMap[dim.Name] = dim.Id;
				foreach (var dim in dataset.Dimensions)
					dimensionIdsMap[dim.Id.Replace('-', '_')] = dim.Id;
				foreach (var dim in dataset.Dimensions)
					dimensionIdsMap[dim.Id] = dim.Id;
			}
			_dimensionIdsMap = dimensionIdsMap;

			Init(request);
		}

		public string TimeRange { get; set; }

		public IReadOnlyDictionary<string, string> DimensionIdsMap
		{
			get
			{
				return _dimensionIdsMap;
			}
		}

		public IEnumerable<string> Frequencies
		{
			get
			{
				return _frequencies;
			}
			set
			{
				_frequencies = value;
			}
		}

		public IEnumerable<string> this[string dimension]
		{
			get
			{
				var dimId = GetDimensionId(dimension);
				if (!string.IsNullOrEmpty(dimId))
				{
					Dictionary<string, string> keys;
					if (_dimensions.TryGetValue(dimId, out keys))
						return keys.Keys;
				}
				return null;
			}
			set
			{
				var dimId = GetDimensionId(dimension);
				if (!string.IsNullOrEmpty(dimId))
				{
					if (value == null)
						_dimensions.Remove(dimId);
					else
					{
						Dictionary<string, string> keys;
						if (!_dimensions.TryGetValue(dimId, out keys))
						{
							keys = new Dictionary<string, string>();
							_dimensions[dimId] = keys;
						}
						foreach (var key in value)
							keys[key] = "";
					}
				}
			}
		}

		private string GetDimensionId(string dimension)
		{
			string id;
			_dimensionIdsMap.TryGetValue(dimension, out id);
			return id;
		}

		public Dictionary<string, string> GetDimensionMembersMapping(string dimension)
		{
			Dictionary<string, string> result;
			_dimensions.TryGetValue(dimension, out result);
			return result;
		}

		private void Init(IEnumerable<KeyValuePair<string, object>> request)
		{
			foreach (var pair in request)
			{
				if (string.Equals(pair.Key, "TimeRange", StringComparison.OrdinalIgnoreCase))
					TimeRange = Convert.ToString(pair.Value);
				else
				{
					var val = pair.Value as IEnumerable<string>;
					if (val == null)
					{
						var str = Convert.ToString(pair.Value);
						if (str != null)
							val = str.Split(';');
					}
					if (string.Equals(pair.Key, "Frequency", StringComparison.OrdinalIgnoreCase))
						Frequencies = val;
					else
						this[pair.Key] = val;
				}
			}
		}

		public PivotRequest GetRequest()
		{
			var request = new PivotRequest();
			request.Dataset = _dataset.Id;
			if (_frequencies != null)
				request.Frequencies = _frequencies.ToList();

			var timeItem = new PivotRequestItem { DimensionId = "Time" };
			if (string.IsNullOrEmpty(TimeRange))
			{
				timeItem.UiMode = "allData";
				timeItem.Members = null;
			}
			else
			{
				timeItem.UiMode = "range";
				timeItem.Members.Add(TimeRange);
			}
			request.Header.Add(timeItem);

			foreach (var dimPair in _dimensions)
			{
				var dimId = dimPair.Key;
				var dimKeys = dimPair.Value;
				if (dimKeys == null || dimKeys.Count == 0)
					continue;

				var dimMeta = _client.GetDatasetDimension(_dataset.Id, dimId).GetAwaiter().GetResult();
				object idVal;
				string keyVal;
				foreach (var member in dimMeta.Items)
				{
					if (member.Fields != null && member.Fields.TryGetValue("id", out idVal) && idVal != null && dimKeys.ContainsKey(idVal.ToString()))
						keyVal = idVal.ToString();
					else if (dimKeys.ContainsKey(member.Name))
						keyVal = member.Name;
					else if (dimKeys.ContainsKey(member.Key.ToString()))
						keyVal = member.Key.ToString();
					else
						keyVal = null;

					if (!string.IsNullOrEmpty(keyVal))
						dimKeys[keyVal] = member.Key;
				}

				PivotRequestItem dimItem = null;
				foreach (var memberKey in dimKeys.Values)
				{
					if (memberKey != "")
					{
						if (dimItem == null)
						{
							dimItem = new PivotRequestItem { DimensionId = dimId };
							request.Stub.Add(dimItem);
						}
						dimItem.Members.Add(memberKey);
					}
				}
			}

			return request;
		}
	}
}
