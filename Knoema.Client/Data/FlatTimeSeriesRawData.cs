using System.Collections.Generic;

namespace Knoema.Data
{
	public class FlatTimeSeriesRawData : TimeSeriesRawData
	{
		public IList<DataItem> Data { get; set; }
	}
}
