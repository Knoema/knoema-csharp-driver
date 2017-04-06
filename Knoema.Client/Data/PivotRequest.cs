using System.Collections.Generic;

namespace Knoema.Data
{
	public class PivotRequest
	{
		public List<PivotRequestItem> Header { get; set; }
		public List<PivotRequestItem> Stub { get; set; }
		public List<PivotRequestItem> Filter { get; set; }
		public List<string> Frequencies { get; set; }
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
