using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knoema.Meta
{
	public class Dimension
	{
		public int Key { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsGeo { get; set; }
		public string DatasetId { get; set; }
		public IEnumerable<DimensionMember> Items { get; set; }
	}
}
