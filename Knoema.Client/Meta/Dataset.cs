using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knoema.Meta
{
	public class Dataset
	{
		public string Id { get; set; }
		public string Ref { get; set; }
		public DateTime? PublicationDate { get; set; }
		public string Title { get; set; }
		public IEnumerable<Dimension> Dimensions { get; set; }
	}
}
