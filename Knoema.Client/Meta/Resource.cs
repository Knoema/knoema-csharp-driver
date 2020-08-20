using System;
using System.Collections.Generic;

namespace Knoema.Meta
{
	public class Resource
	{
		public int Key { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public bool Public { get; set; }
		public string Url { get; set; }
		public DateTime? Created { get; set; }
		public DateTime? Updated { get; set; }
		public IList<string> Tags { get; set; }
	}
}
