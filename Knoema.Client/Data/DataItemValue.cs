using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public enum DataItemType
	{
		Detail,
		Time,
		Measure
	}

	[JsonConverter(typeof(DataItemValueConverter))]
	public abstract class DataItemValue
	{
		[JsonExtensionData]
		public IDictionary<string, object> Fields { get; set; }
		public string Name { get; set; }
		public DataItemType Type { get; set; }
	}

	public class DataItemDetail : DataItemValue
	{
		public string Value { get; set; }
	}

	public class DataItemTime : DataItemValue
	{
		public DateTime Date { get; set; }
		public string Frequency { get; set; }
	}

	public class DataItemMeasure : DataItemValue
	{
		public object Value { get; set; }
		public string Unit { get; set; }
	}
}
