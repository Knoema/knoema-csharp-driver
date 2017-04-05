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
			if (verifyResult.MetadataDetails != null)
			{
				Name = verifyResult.MetadataDetails.DatasetName;
				Description = verifyResult.MetadataDetails.Description;
				Source = verifyResult.MetadataDetails.Source;
				Url = verifyResult.MetadataDetails.DatasetRef;
				PubDate = verifyResult.MetadataDetails.PublicationDate;
				AccessedOn = verifyResult.MetadataDetails.AccessedOn;
			}
		}

		public string DatasetId { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public DateTime? PubDate { get; set; }
		public DateTime? AccessedOn { get; set; }
		public string Source { get; set; }
		public IList<object> Columns { get; set; }
		public FileProperties FileProperty { get; set; }
		public object UploadFormatType { get; set; }
		public string Url { get; set; }
		public FlatDatasetUpdateOptions FlatDSUpdateOptions { get; set; }
		public Dictionary<string, string> MetadataFieldValues { get; set; }
		public int? StartAtRow { get; set; }
	}
}
