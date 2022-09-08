using System.Collections.Generic;

namespace Knoema.Upload
{
	public class DimensionChanges
	{
		public List<string> AddedFields { get; set; }
		public long AddedMembers { get; set; }
		public long UpdatedMembers { get; set; }
		public long TotalMembersInUpdate { get; set; }

		public DimensionChanges()
		{
			AddedFields = new List<string>();
		}
	}
}
