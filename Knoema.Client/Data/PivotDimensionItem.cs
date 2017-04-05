using System.Collections.Generic;

namespace Knoema.Data
{
	public class PivotDimensionItem
	{
		public string DimensionId { get; set; }
		public List<object> Members { get; private set; }

		public PivotDimensionItem()
		{
			Members = new List<object>();
		}
	}
}
