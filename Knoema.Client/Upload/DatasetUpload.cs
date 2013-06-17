using System.Collections.Generic;

namespace Knoema.Upload
{
	public class DatasetUpload
	{
		public string DatasetId { get; set; }
		public string Name { get; set; }
		public List<object> Columns { get; set; }
		public FileProperties FileProperty { get; set; }
		public object UploadFormatType { get; set; }
		public FlatDatasetUpdateOptions FlatDSUpdateOptions { get; set; }
	}
}
