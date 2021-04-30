using System.Collections.Generic;

namespace Knoema.Data
{
	public class DimensionRequestItem
	{
		public string DimensionId { get; set; }
		public List<string> Members { get; set; }

		public DimensionRequestItem()
		{
			Members = new List<string>();
		}
	}
}
