using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Knoema.Data
{
	public enum AttributeType
	{
		Base,
		DateTime,
		Unit
	}

	public abstract class AttributeFlat
	{
		[JsonExtensionData]
		public Dictionary<string, object> Fields { get; set; }
		public string Name { get; set; }
		public AttributeType Type { get; set; }
	}

	public class AttributeBase : AttributeFlat
	{
		public object Value { get; set; }
	}

	public class AttributeDate : AttributeFlat
	{
		public DateTime Date { get; set; }
		public string Frequency { get; set; }
	}

	public class AttributeUnit : AttributeFlat
	{
		public object Value { get; set; }
		public string Unit { get; set; }
	}
}
