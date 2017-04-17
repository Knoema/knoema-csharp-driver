using System.Collections.Generic;

namespace Knoema.Data
{
	public class FlatTimeSeriesRawData : TimeSeriesRawData
	{
		public IEnumerable<DataItem> Data { get; set; }
	}
}
