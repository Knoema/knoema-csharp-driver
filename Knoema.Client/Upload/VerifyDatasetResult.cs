using System.Collections.Generic;

namespace Knoema.Upload
{
	public class VerifyDatasetResult
	{
		public string Status { get; set; }
		public List<string> Errors { get; set; }
	}
}