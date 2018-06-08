using Newtonsoft.Json;

namespace Knoema.Meta
{
	public class DatasetUnloadTaskResult : TaskResult
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public DatasetUnloadTaskResultData Data { get; set; }
	}
}
