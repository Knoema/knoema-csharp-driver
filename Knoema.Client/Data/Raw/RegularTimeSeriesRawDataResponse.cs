using System;
using System.Collections.Generic;
using System.Linq;

namespace Knoema.Data
{
	public class RegularTimeSeriesRawDataResponse: TimeSeriesRawDataResponse
	{
		public IEnumerable<RegularTimeSeriesRawData> Data { get; set; }

		protected override IEnumerable<TimeSeriesRawData> GetData()
		{
			return Data?.Cast<TimeSeriesRawData>();
		}
	}
}
