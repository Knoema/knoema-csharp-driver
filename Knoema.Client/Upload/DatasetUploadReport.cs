using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Upload
{
	public class DatasetUploadReport
	{
		public long AddedDataPoints { get; set; }
		public long UpdatedDataPoints { get; set; }
		public long DeletedDataPoints { get; set; }
		public long AddedMetadataElements { get; set; }

		[Obsolete("Use MetadataChanges.", false)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<UploadedDimensionDetails> MetaDataDetails { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public MetadataChanges MetadataChanges { get; set; }

		public long TotalRecords { get; set; }
		public bool IsFlatDataset { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<string> UploadComments { get; set; }
		public bool IsKnoxDataOverwritten { get; set; }
	}
}
