using System;

namespace Knoema.Upload
{
	class VerifyDatasetRequest
	{
		public string Id { get; set; }
		public DateTime? PublicationDate { get; set; }
		public string Source { get; set; }
		public string RefUrl { get; set; }
	}
}
