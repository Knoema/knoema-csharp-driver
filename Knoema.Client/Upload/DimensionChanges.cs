using System.Collections.Generic;

namespace Knoema.Upload
{
	public class DimensionChanges
	{
		public IList<string> AddedFields { get; set; }
		public int AddedMembers { get; set; }
		public int UpdatedMembers { get; set; }
		public int TotalMembersInUpdate { get; set; }

		public DimensionChanges()
		{
			AddedFields = new List<string>();
		}
	}
}
