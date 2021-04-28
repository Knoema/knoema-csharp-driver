using System.Collections.Generic;

namespace Knoema.Meta
{
	public class DimensionMember
	{
		public virtual string Key { get; set; }
		public virtual string Name { get; set; }
		public virtual int Level { get; set; }
		public virtual bool HasData { get; set; }
		public virtual Dictionary<string, object> Fields { get; set; }
	}
}
