using Newtonsoft.Json;

namespace Knoema.Upload
{
	public class FileProperties
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int Size { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Location { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string Type { get; set; }
	}
}
