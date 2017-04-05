using System.Collections.Generic;

namespace Knoema.Data
{
	public class PivotRequest
	{
		public IList<PivotRequestItem> Header { get; set; }
		public IList<PivotRequestItem> Stub { get; set; }
		public IList<PivotRequestItem> Filter { get; set; }
		public IList<string> Frequencies { get; set; }
		public string Dataset { get; set; }

		public PivotRequest()
		{
			Header = new List<PivotRequestItem>();
			Stub = new List<PivotRequestItem>();
			Filter = new List<PivotRequestItem>();
			Frequencies = new List<string>();
		}
	}
}
