namespace Knoema.Meta
{
	public class ResourceUsage
	{
		public string CommunityId { get; }
		public string CommunityName { get; }
		public bool IsShortcut { get; }
		public bool Atlas { get; }
		public int Pages { get; }
		public bool IsAuto { get; set; }
	}
}
