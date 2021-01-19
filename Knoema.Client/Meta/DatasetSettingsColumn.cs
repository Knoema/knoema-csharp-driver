using System.Collections.Generic;

namespace Knoema.Meta
{
	public class DatasetSettingsColumn
	{
		public int Id { get; set; }
		public string IdName { get; set; }
		public string Name { get; set; }
		public ColumnStatus Status { get; set; }
		public ColumnType Type { get; set; }
		public List<string> Members { get; set; }
		public string GroupedTo { get; set; }
		public ColumnGroupingType GroupedAs { get; set; }
		public bool IsDimNamesclear { get; set; }
		public bool HideInPreview { get; set; }
	}
}
