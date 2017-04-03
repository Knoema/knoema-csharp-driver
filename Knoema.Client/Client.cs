using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Knoema.Data;
using Knoema.Meta;
using Knoema.Upload;

namespace Knoema
{
	public class Client : ClientBase
	{
		private const string _apiDataPivot = "/api/1.0/data/pivot/";
		private const string _api11DataPivot = "/api/1.1/data/pivot/";
		private const string _apiDataMultipivot = @"/api/1.0/data/multipivot";
		private const string _apiMetaDataset = "/api/1.0/meta/dataset/{0}";
		private const string _apiMetaDatasetList = "/api/1.0/meta/dataset";
		private const string _apiMetaDatasetDimension = "/api/1.0/meta/dataset/{0}/dimension/{1}";
		private const string _apiMetaDatasetDateRange = "/api/1.0/meta/dataset/{0}/daterange";
		private const string _apiMetaVerifyDataset = "/api/1.0/meta/verifydataset";
		private const string _apiUploadPost = "/api/1.0/upload/post";
		private const string _apiUploadVerify = "/api/1.0/upload/verify";
		private const string _apiUploadSave = "/api/1.0/upload/save";
		private const string _apiUploadStatus = "/api/1.0/upload/status";
		private const string _apiDataRaw = "/api/1.0/data/raw/";

		public Client(string host)
			: base(host)
		{ }

		public Client(string host, string token)
			: base(host, token)
		{ }

		public Client(string host, string appId, string appSecret)
			: base(host, appId, appSecret)
		{ }

		public Task<Dataset> GetDataset(string datasetId)
		{
			return _accessor.Get<Dataset>(string.Format(_apiMetaDataset, datasetId));
		}

		public Task<Dimension> GetDatasetDimension(string datasetId, string dimensionId)
		{
			return _accessor.Get<Dimension>(string.Format(_apiMetaDatasetDimension, datasetId, dimensionId));
		}

		public Task<List<Dataset>> ListDatasets(string source = null, string topic = null, string region = null)
		{
			if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(topic) && string.IsNullOrEmpty(region))
				return _accessor.Get<List<Dataset>>(_apiMetaDatasetList);

			return _accessor.Post<Dictionary<string, string>, List<Dataset>>(_apiMetaDatasetList,
				new Dictionary<string, string>()
					{
						{"source", source},
						{"topic", topic},
						{"region", region}
					});
		}

		public Task<PivotResponse> GetPivotData(PivotRequest pivot)
		{
			return _accessor.Post<PivotRequest, PivotResponse>(_apiDataPivot, pivot);
		}

		public Task<List<PivotResponse>> GetMultipivotData(List<PivotRequest> pivots)
		{
			return _accessor.Post<List<PivotRequest>, List<PivotResponse>>(_apiDataMultipivot, pivots);
		}

		public Task<PivotResponseWithToken> GetPivotDataBegin(PivotRequestWithAdvancedHeader request)
		{
			return _accessor.Post<PivotRequestWithAdvancedHeader, PivotResponseWithToken>(_apiDataRaw, request);
		}

		public Task<PivotResponseWithToken> GetPivotDataContinuation(string token)
		{
			var parameters = HttpUtility.ParseQueryString(string.Empty);
			parameters.Add("continuationToken", token);
			return _accessor.Get<PivotResponseWithToken>(_apiDataRaw, parameters);
		}

		public Task<FlatResponseWithToken> GetFlatDataBegin(PivotRequestWithAdvancedHeader request)
		{
			return _accessor.Post<PivotRequestWithAdvancedHeader, FlatResponseWithToken>(_apiDataRaw, request);
		}

		public Task<FlatResponseWithToken> GetFlatDataContinuation(string token)
		{
			var parameters = HttpUtility.ParseQueryString(string.Empty);
			parameters.Add("continuationToken", token);
			return _accessor.Get<FlatResponseWithToken>(_apiDataRaw, parameters);
		}

		public Task<PostResult> UploadPost(string fileName)
		{
			var fileInfo = new FileInfo(fileName);
			var fileStream = fileInfo.OpenRead();
			
			var content = new MultipartFormDataContent();
			content.Add(new StreamContent(fileStream), "\"file\"", "\"" + fileInfo.Name + "\"");

			var uri = _accessor.GetUri(_apiUploadPost);
			var message = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };
			return _accessor.Access<PostResult>(message);
			
		}

		public Task<VerifyResult> UploadVerify(string filePath, string existingDatasetIdToModify = null)
		{
			var parameters = HttpUtility.ParseQueryString(string.Empty);
			parameters.Add("filePath", filePath);
			parameters.Add("datasetId", existingDatasetIdToModify);
			return _accessor.Get<VerifyResult>(_apiUploadVerify, parameters);
		}

		public Task<UploadResult> UploadSubmit(DatasetUpload upload)
		{
			return _accessor.Post<DatasetUpload, UploadResult>(_apiUploadSave, upload);
		}

		public Task<UploadResult> UploadStatus(int uploadId)
		{
			var parameters = HttpUtility.ParseQueryString(string.Empty);
			parameters.Add("id", uploadId.ToString());
			return _accessor.Get<UploadResult>(_apiUploadStatus, parameters);
		}

		public Task<VerifyDatasetResult> VerifyDataset(string id, DateTime? publicationDate = null, string source = null, string refUrl = null)
		{
			var request = new VerifyDatasetRequest()
			{
				Id = id,
				PublicationDate = publicationDate,
				Source = source,
				RefUrl = refUrl
			};
			return _accessor.Post<VerifyDatasetRequest, VerifyDatasetResult>(_apiMetaVerifyDataset, request);
		}

		public Task<UploadResult> UploadDataset(string filename, string datasetName)
		{
			var postResult = UploadPost(filename).Result;
			if (!postResult.Successful)
				return null;

			var verifyResult = UploadVerify(postResult.Properties.Location).Result;
			if (!verifyResult.Successful)
				return null;

			var upload = new DatasetUpload()
			{
				Name = datasetName,
				UploadFormatType = verifyResult.UploadFormatType,
				Columns = verifyResult.Columns,
				FlatDSUpdateOptions = verifyResult.FlatDSUpdateOptions,
				FileProperty = postResult.Properties
			};

			var result = UploadSubmit(upload).Result;
			while (UploadStatus(result.Id).Result.Status == "in progress")
				System.Threading.Thread.Sleep(5000);

			return UploadStatus(result.Id);
		}

		public Task<DateRange> GetDatasetDateRange(string datasetId)
		{
			return _accessor.Get<DateRange>(string.Format(_apiMetaDatasetDateRange, datasetId));
		}
	}
}
