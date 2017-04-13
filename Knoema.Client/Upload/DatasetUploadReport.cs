using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Upload
{
	public class DatasetUploadReport
	{
		public int AddedDataPoints { get; set; }
		public int UpdatedDataPoints { get; set; }
		public int DeletedDataPoints { get; set; }
		public int AddedMetadataElements { get; set; }

		[Obsolete("Use MetadataChanges.", false)]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<UploadedDimensionDetails> MetaDataDetails { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public MetadataChanges MetadataChanges { get; set; }

		public int TotalRecords { get; set; }
		public bool IsFlatDataset { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<string> UploadComments { get; set; }
		public bool IsKnoxDataOverwritten { get; set; }
	}
}
