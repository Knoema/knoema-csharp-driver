﻿using System.Collections.Generic;

namespace Knoema.Meta
{
	public class Dimension
	{
		public int Key { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsGeo { get; set; }
		public string DatasetId { get; set; }
		public IList<DimensionMember> Items { get; set; }
		public IList<DimensionField> Fields { get; set; }
	}
}
