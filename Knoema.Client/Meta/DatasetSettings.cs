using System.Collections.Generic;

namespace Knoema.Meta
{
	public class DatasetSettings
	{
		public bool ColumnStoreFlag { get; set; }
		public bool UseDataSessionFlag { get; set; }
		public bool UseSnowflakeFlag { get; set; }
		public IEnumerable<DatasetSettingsColumn> Columns { get; set; }
	}
}
