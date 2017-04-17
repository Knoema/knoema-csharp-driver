using System;

namespace Knoema.Search
{
	[Flags]
	public enum SearchScope
	{
		Timeseries = 0x8,
		NamedEntity = 0x100,
		Atlas = 0x200,
		Semantic = 0x800
	}
}
