using Newtonsoft.Json;

namespace Knoema.Meta
{
	public class TaskResponse
	{
		public int? TaskKey { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public TaskResponse ProxyData { get; set; }
	}
}
