using System;
using System.Collections.Generic;
using System.Text;
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
	}
}
