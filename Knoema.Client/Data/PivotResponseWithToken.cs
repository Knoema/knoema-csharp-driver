using System.Collections.Generic;

namespace Knoema.Data
{
	public class PivotResponseWithToken
	{
		public string ContinuationToken { get; set; }
		public List<PivotResponseWithTokenData> Data { get; set; }
	}
}
