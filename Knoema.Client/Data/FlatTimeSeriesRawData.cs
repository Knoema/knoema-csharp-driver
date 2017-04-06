using System.Collections.Generic;

namespace Knoema.Data
{
	public class FlatTimeSeriesRawData : TimeSeriesRawData
	{
		public List<DataItem> Data { get; set; }
	}
}
