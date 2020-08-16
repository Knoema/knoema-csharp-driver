using System.Collections.Generic;

namespace Knoema.Data
{
	public class StreamingDataResponse<T>
	{
		public string ContinuationToken { get; set; }
		public IEnumerable<T> Data { get; set; }
	}
}
