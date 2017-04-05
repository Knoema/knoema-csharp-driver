using System.Collections.Generic;

namespace Knoema.Meta
{
	public class DimensionMember
	{
		public int Key { get; set; }
		public string Name { get; set; }
		public int Level { get; set; }
		public bool HasData { get; set; }
		public Dictionary<string, object> Fields { get; set; }
	}
}
