using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Upload
{
	public class VerifyResult
	{
		public bool Successful { get; set; }

		public string FilePath { get; set; }

		public List<string> ErrorList { get; set; }

		public object UploadFormatType { get; set; }

		public List<object> Columns { get; set; }

		public DatasetUploadDetails MetadataDetails { get; set; }

		public FlatDatasetUpdateOptions FlatDSUpdateOptions { get; set; }
	}
}
