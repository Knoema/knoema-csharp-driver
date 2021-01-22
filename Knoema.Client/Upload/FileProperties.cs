using Newtonsoft.Json;

namespace Knoema.Upload
{
	public enum FileBucket
	{
		Temp = 0,
		Uploads = 1
	}

	public class FileProperties
	{
		public int Size { get; set; }
		public string Name { get; set; }
		public string Location { get; set; }
		public string Type { get; set; }
		public FileBucket? FileBucket { get; set; }
	}
}
