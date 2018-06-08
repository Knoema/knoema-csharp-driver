using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Knoema.Meta
{
	public class TaskResult
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public TaskStatus Status { get; set; }

		public string Message { get; set; }
	}
}
