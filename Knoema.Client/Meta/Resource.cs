using System;
using System.Collections.Generic;

namespace Knoema.Meta
{
	public class Resource
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string RawDescription { get; set; }
		public string Type { get; set; }
		public string ResourceUrl { get; set; }
		public string EditUrl { get; set; }
		public string EmbedUrl { get; set; }
		public string ThumbnailUrl { get; set; }
		public string BigThumbnailUrl { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }
		public ResourceOwner Owner { get; set; }
		public bool Public { get; set; }
		public IReadOnlyCollection<string> Tags { get; set; }
	}
}
