using Newtonsoft.Json;

namespace Knoema.Upload
{
	public class PostResult
	{
		public bool Successful { get; set; }

		public FileProperties Properties { get; set; }

		public string Error { get; set; }
	}
}
