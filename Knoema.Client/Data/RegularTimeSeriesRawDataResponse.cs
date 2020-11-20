using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Knoema.Data
{
	public class RegularTimeSeriesRawDataResponse
	{
		public TimeSeriesRawDataDescriptor Descriptor { get; set; }
		public string ContinuationToken { get; set; }
		public IEnumerable<RegularTimeSeriesRawData> Data { get; set; }
	}
}
