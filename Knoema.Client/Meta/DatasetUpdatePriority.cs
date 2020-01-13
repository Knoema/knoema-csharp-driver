using System.ComponentModel;

namespace Knoema.Meta
{
	public enum DatasetUpdatePriority
	{
		High = 0,
		Medium = 1,
		Low = 2,
		[Description("On-demand")]
		OnDemand = 3,
		Discontinued = 4
	}
}
