using System.Collections.Generic;

namespace Knoema.Data
{
	public class RegularTimeSeriesRawDataResponse
	{
		public string ContinuationToken { get; set; }
		public List<RegularTimeSeriesRawData> Data { get; set; }
	}
}
