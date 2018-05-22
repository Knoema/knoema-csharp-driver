using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Knoema.Meta
{
	[JsonConverter(typeof(DatasetConverter))]
	public class Dataset
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Ref { get; set; }
		public string Type { get; set; }
		public string HasGeoDimension { get; set; }
		public string RegionDimensionId { get; set; }
		public DateTime? PublicationDate { get; set; }
		public IEnumerable<Dimension> Dimensions { get; set; }
		public IEnumerable<Column> Columns { get; set; }
		public IEnumerable<TimeSeriesAttribute> TimeSeriesAttributes { get; set; }
		public VerificationStatus Status { get; set; }
		[JsonIgnore] // dodging from ambiguous between json object property
		public DatasetSettings Settings { get; set; }
	}
}
