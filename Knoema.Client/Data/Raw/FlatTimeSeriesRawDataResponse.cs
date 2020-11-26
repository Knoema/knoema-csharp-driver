using System.Collections.Generic;
using System.Linq;

namespace Knoema.Data
{
	public class FlatTimeSeriesRawDataResponse : TimeSeriesRawDataResponse
	{
		public IEnumerable<FlatTimeSeriesRawData> Data { get; set; }
		protected override IEnumerable<TimeSeriesRawData> GetData()
		{
			return Data?.Cast<TimeSeriesRawData>();
		}
	}
}
