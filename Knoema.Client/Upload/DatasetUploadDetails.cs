using System;

namespace Knoema.Upload
{
	public class DatasetUploadDetails
	{
		public string DatasetId { get; set; }
		public string DatasetName { get; set; }
		public string Source { get; set; }
		public string Description { get; set; }
		public string DatasetRef { get; set; }
		public DateTime? PublicationDate { get; set; }
		public DateTime? AccessedOn { get; set; }
	}
}
