using System.Collections.Generic;

namespace Knoema.Upload
{
	public class VerifyDatasetResult
	{
		public string Status { get; set; }
		public IList<string> Errors { get; set; }
	}
}