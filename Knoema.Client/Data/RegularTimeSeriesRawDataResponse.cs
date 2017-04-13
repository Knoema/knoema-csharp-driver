using System.Collections.Generic;

namespace Knoema.Data
{
	public class RegularTimeSeriesRawDataResponse
	{
		public string ContinuationToken { get; set; }
		public IEnumerable<RegularTimeSeriesRawData> Data { get; set; }
	}
}
