using System;
using System.Collections.Generic;
using System.Text;

namespace Knoema.Meta
{
	public class DataOpsDatasetViewModel
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public int PagesCount { get; set; }

		public ICollection<string> Tags { get; set; }

		public HashSet<string> Communities { get; set; }

		public int DatasetViews { get; set; }

		public IReadOnlyCollection<string> Frequencies { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public bool? IsAutoUpdated { get; set; }

		public bool IsAtlas { get; set; }

		public DatasetUpdatePriority? UpdatePriority { get; }

		public string Ref { get; }

		public DateTime? PublicationDate { get; set; }

		public DateTime? AccessedOn { get; set; }

		public DateTime? NextReleaseDate { get; set; }

		public DateTime? ExpectedUpdateDate { get; set; }

		public DateTime LastUpdatedOn { get; set; }

		public string SourceName { get; set; }

		public string TermsOfUseLink { get; set; }
	}
}
