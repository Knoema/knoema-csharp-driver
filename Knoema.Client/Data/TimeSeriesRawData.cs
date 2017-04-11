using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public abstract class TimeSeriesRawData
	{
		[JsonExtensionData]
		private Dictionary<string, DimensionItem> _dimensions;

		public List<DimensionItem> Dimensions { get; set; }

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			Dimensions = _dimensions.Select(pair => 
			{
				var value = pair.Value;
				value.DimensionId = pair.Key;
				return value;
			}).ToList();
		}
	}
}
