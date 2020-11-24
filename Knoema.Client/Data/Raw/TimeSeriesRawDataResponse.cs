using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Knoema.Data
{
	public abstract class TimeSeriesRawDataResponse
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public TimeSeriesRawDataDescriptor Descriptor { get; set; }

		public string ContinuationToken { get; set; }

		public abstract IEnumerable<TimeSeriesRawData> GetData();

		public static JsonSerializerSettings GetSerializerSettings()
		{
			return new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
		}

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			var detailColumns = Descriptor.DetailColumns?.Select(c => c.Name).ToArray();
			foreach (var item in GetData() ?? Enumerable.Empty<TimeSeriesRawData>())
			{
				item.InitAfterDeserialized(detailColumns);
			}
		}
	}
}
