namespace Knoema.Meta
{
	public class DimensionField
	{
		public int Key { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public DimensionFieldType Type { get; set; }
		public string Locale { get; set; }
		public int? BaseKey { get; set; }
		public bool IsSystemField { get; set; }
	}
}
