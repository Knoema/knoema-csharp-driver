using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Upload
{
	public class VerifyResult
	{
		public bool Successful { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string FilePath { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<string> ErrorList { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public object UploadFormatType { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<object> Columns { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public FlatDatasetUpdateOptions FlatDSUpdateOptions { get; set; }
	}
}
