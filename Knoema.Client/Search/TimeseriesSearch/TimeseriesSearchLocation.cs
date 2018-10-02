using System;
using System.Collections.Generic;

namespace Knoema.Search.TimeseriesSearch
{
	public class TimeseriesSearchLocation
	{
		public int DatasetKey { get; set; }
		public DateTime Expires { get; set; }
		public string Key { get; set; }
		public IEnumerable<TimeseriesSearchPositionItem> Position { get; set; }
	}
}