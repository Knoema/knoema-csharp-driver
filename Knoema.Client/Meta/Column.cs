namespace Knoema.Meta
{
	public enum ColumnType
	{
		Date = 0,
		Text = 1,
		Number = 2,
		Location = 3,
		Currency = 4,
		Latitude = 7,
		Longitude = 8
	}

	public enum ColumnStatus
	{
		Dimension = 0,
		Measure = 1,
		Date = 2,
		Detail = 3,
		Attribute = 4,
		Frequency = 5,
		Scale = 6,
		Unit = 7
	}

	public enum ColumnGroupingType
	{
		Property = 0,
		Hierarchy = 1
	}

	public class Column
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
		public ColumnType Type { get; set; }
		public ColumnStatus Status { get; set; }
		public string DimensionId { get; set; }
		public string DimensionFieldId { get; set; }
	}
}
