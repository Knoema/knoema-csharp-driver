using System.Collections.Generic;

namespace Knoema.Search
{
	public class RegionLink
	{
		public int Key { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string IdFromName { get; set; }
		public IEnumerable<string> Parents { get; set; }
	}
}
