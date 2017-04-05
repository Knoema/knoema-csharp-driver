using System.Collections.Generic;

namespace Knoema.Data
{
	public class FlatTimeSeriesRawDataResponse
	{
		public string ContinuationToken { get; set; }
		public List<FlatTimeSeriesRawData> Data { get; set; }
	}
}
