using System;
using System.Collections.Generic;

namespace Knoema.Upload
{
	public class DatasetUpload
	{
		public DatasetUpload() { }

		public DatasetUpload(VerifyResult verifyResult, PostResult postResult, string datasetId = null)
		{
			DatasetId = datasetId;
			FileProperty = postResult.Properties;
			UploadFormatType = verifyResult.UploadFormatType;
			Columns = verifyResult.Columns;
			FlatDSUpdateOptions = verifyResult.FlatDSUpdateOptions;
			RegularDSUpdateOptions = verifyResult.RegularDSUpdateOptions;
			if (verifyResult.MetadataDetails != null)
			{
				Name = verifyResult.MetadataDetails.DatasetName;
				Description = verifyResult.MetadataDetails.Description;
				Source = verifyResult.MetadataDetails.Source;
				Url = verifyResult.MetadataDetails.DatasetRef;
				PubDate = verifyResult.MetadataDetails.PublicationDate;
				AccessedOn = verifyResult.MetadataDetails.AccessedOn;
				NextReleaseDate = verifyResult.MetadataDetails.NextReleaseDate;
			}
		}

		public string DatasetId { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public DateTime? PubDate { get; set; }
		public DateTime? AccessedOn { get; set; }
		public DateTime? NextReleaseDate { get; set; }
		public string Source { get; set; }
		public List<object> Columns { get; set; }
		public FileProperties FileProperty { get; set; }
		public object UploadFormatType { get; set; }
		public string Url { get; set; }
		public FlatDatasetUpdateOptions FlatDSUpdateOptions { get; set; }
		public RegularDatasetUpdateOptions? RegularDSUpdateOptions { get; set; }
		public IDictionary<string, string> MetadataFieldValues { get; set; }
		public int? StartAtRow { get; set; }
		public bool Public { get; set; }
	}
}
