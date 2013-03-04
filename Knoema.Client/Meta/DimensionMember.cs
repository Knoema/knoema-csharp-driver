using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knoema.Meta
{
	public class DimensionMember
	{
		public virtual int Key { get; set; }
		public virtual string Name { get; set; }
		public virtual int Level { get; set; }
		public virtual bool HasData { get; set; }
	}
}
