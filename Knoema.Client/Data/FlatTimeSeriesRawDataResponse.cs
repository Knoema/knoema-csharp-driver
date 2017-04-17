using System.Collections.Generic;

namespace Knoema.Data
{
	public class FlatTimeSeriesRawDataResponse
	{
		public string ContinuationToken { get; set; }
		public IEnumerable<FlatTimeSeriesRawData> Data { get; set; }
	}
}
