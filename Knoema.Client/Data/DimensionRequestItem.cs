using System.Collections.Generic;

namespace Knoema.Data
{
	public class DimensionRequestItem
	{
		public string DimensionId { get; set; }
		public List<int> Members { get; set; }

		public DimensionRequestItem()
		{
			Members = new List<int>();
		}
	}
}
