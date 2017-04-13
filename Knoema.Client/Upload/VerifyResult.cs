using System.Collections.Generic;

namespace Knoema.Upload
{
	public class VerifyResult
	{
		public bool Successful { get; set; }
		public string FilePath { get; set; }
		public IEnumerable<string> ErrorList { get; set; }
		public object UploadFormatType { get; set; }
		public IEnumerable<object> Columns { get; set; }
		public DatasetUploadDetails MetadataDetails { get; set; }
		public FlatDatasetUpdateOptions FlatDSUpdateOptions { get; set; }
		public DatasetUploadReport AdvanceReport { get; set; }
	}
}
