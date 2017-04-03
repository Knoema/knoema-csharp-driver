using System.Collections.Generic;

namespace Knoema.Data
{
	public abstract class PivotRequestBase<T>
	{
		public List<T> Header { get; set; }
		public List<PivotRequestItem> Stub { get; set; }
		public List<PivotRequestItem> Filter { get; set; }
		public string Dataset { get; set; }
		public List<string> Frequencies { get; set; }

		protected PivotRequestBase()
		{
			Header = new List<T>();
			Stub = new List<PivotRequestItem>();
			Filter = new List<PivotRequestItem>();
			Frequencies = new List<string>();
		}
	}

	public class PivotRequest : PivotRequestBase<PivotRequestItem>
	{
	}

	public class PivotRequestWithAdvancedHeader : PivotRequestBase<PivotRequestTimeItem>
	{
	}
}
