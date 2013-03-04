using System.Collections.Generic;

namespace Knoema.Data
{
	public class PivotRequestItem
	{
		public string DimensionId { get; set; }
		public IList<object> Members { get; set; }

		public PivotRequestItem()
		{
			Members = new List<object>();
		}
	}
}
