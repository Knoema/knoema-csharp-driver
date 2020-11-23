using System.Collections.Generic;
using System.Linq;

namespace Knoema.Data
{
	public class FlatTimeSeriesRawDataResponse : TimeSeriesRawDataResponse
	{
		public IEnumerable<FlatTimeSeriesRawData> Data { get; set; }
		public override IEnumerable<TimeSeriesRawData> GetData()
		{
			return Data != null ? Data.Cast<TimeSeriesRawData>() : null;
		}
	}
}
