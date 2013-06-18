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
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<UploadedDimensionDetails> MetaDataDetails { get; set; }
		public int TotalRecords { get; set; }
		public bool IsFlatDataset { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<string> UploadComments { get; set; }
	}
}
