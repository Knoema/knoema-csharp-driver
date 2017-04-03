using System.Collections.Generic;

namespace Knoema.Data
{
	public class FlatResponseWithToken
	{
		public string ContinuationToken { get; set; }
		public List<FlatResponseWithTokenData> Data { get; set; }
	}
}
