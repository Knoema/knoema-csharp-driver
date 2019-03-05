using System;

namespace Knoema.Meta
{
	public class MetadataUpdate
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Source { get; set; }
		public string ReferenceUrl { get; set; }
		public DateTime? PublicationDate { get; set; }
		public DateTime? AccessedOn { get; set; }
		public DateTime? NextReleaseDate { get; set; }
	}
}
