namespace Knoema.Meta
{
	public class DatasetSource
	{
		public string Id { get; set; }
		public string Url { get; set; }
		public string Name { get; set; }
		public string LocalizedName { get; set; }
		public bool IsVerified { get; set; }
		public string LicenseTypeName { get; set; }
		public string TermsOfUseLink { get; set; }
	}
}
