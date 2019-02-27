using System.Collections.Generic;

namespace Knoema.Upload
{
	public class RequestStatusResult
	{
		public string Status { get; set; }
		public List<string> Errors { get; set; }
	}
}