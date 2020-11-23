using System;
using System.Collections.Generic;
using System.Linq;

namespace Knoema.Data
{
	public class RegularTimeSeriesRawDataResponse: TimeSeriesRawDataResponse
	{
		public IEnumerable<RegularTimeSeriesRawData> Data { get; set; }

		public override IEnumerable<TimeSeriesRawData> GetData()
		{
			if (Data == null)
				return null;

			if (Descriptor == null)
				return Data;

			if (Data.All(d => d.DetailColumnsData == null))
				return Data;

			var columns = Descriptor.DetailColumns.Select(c => c.Name);
			var keysToRemove = Data.First().DetailColumnsData.Keys.Where(k => !columns.Contains(k, StringComparer.OrdinalIgnoreCase));

			foreach (var data in Data)
			{
				foreach (var key in keysToRemove)
				{
					data.DetailColumnsData.Remove(key);
				}
			}

			return Data.Cast<TimeSeriesRawData>();
		}
	}
}
