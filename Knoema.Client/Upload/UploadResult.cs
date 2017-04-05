using System.Collections.Generic;

namespace Knoema.Upload
{
	public class UploadResult
	{
		public int Id { get; set; }
		public string DatasetId { get; set; }
		public string Status { get; set; }
		public IList<string> Errors { get; set; }
		public string Url { get; set; }
		public DatasetUploadReport Report { get; set; }

		public UploadResult()
		{
			Errors = new List<string>();
		}
	}
}
