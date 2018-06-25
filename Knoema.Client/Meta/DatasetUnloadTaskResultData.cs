using System.Collections.Generic;

namespace Knoema.Meta
{
	public class DatasetUnloadTaskResultData
	{
		public class FileInfo
		{
			public string Name { get; set; }
			public string Url { get; set; }
			public long Length { get; set; }
		}

		private readonly List<FileInfo> _files;
		public List<FileInfo> Files { get { return _files; } }

		public DatasetUnloadTaskResultData()
		{
			_files = new List<FileInfo>();
		}
	}
}
