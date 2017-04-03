using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public class PivotResponseWithTokenData
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Frequency { get; set; }
		public string Unit { get; set; }
		public float Scale { get; set; }
		public List<object> Values { get; set; }

		[JsonExtensionData]
		public Dictionary<string, object> Dimensions { get; set; }

		public List<PivotDatasetDimension> GetDimensions()
		{
			return Dimensions.Select(pair =>
			{
				var result = JsonConvert.DeserializeObject<PivotDatasetDimension>(pair.Value.ToString());
				result.DimensionId = pair.Key;
				return result;
			}).ToList();
		}
	}
}
