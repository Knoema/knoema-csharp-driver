using System;

namespace Knoema.Search
{
	public class DatasetDescriptor
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Source { get; set; }
		public string DataUrl { get; set; }
		public string AccessedOn { get; set; }
		public string SourceUrl { get; set; }
		public string SourceId { get; set; }
		public DateTime? UpdatedOn { get; set; }
		public string DatasetType { get; set; }
	}
}
