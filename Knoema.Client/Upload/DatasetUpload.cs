using System.Collections.Generic;
using System;

namespace Knoema.Upload
{
	public class DatasetUpload
	{
		public string DatasetId { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public DateTime? PubDate { get; set; }
		public string Source { get; set; }
		public List<object> Columns { get; set; }
		public FileProperties FileProperty { get; set; }
		public object UploadFormatType { get; set; }
		public string Url { get; set; }
		public FlatDatasetUpdateOptions FlatDSUpdateOptions { get; set; }
		public IDictionary<string, string> MetadataFieldValues { get; set; }
	}
}
