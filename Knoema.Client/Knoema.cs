using Knoema.Series;

namespace Knoema
{
	public static class Knoema
	{
		private static Client _client = null;

		public static Client Client
		{
			get
			{
				var result = _client;
				if (result == null)
				{
					result = new Client("knoema.com");
					_client = result;
				}
				return result;
			}
			set
			{
				_client = value;
			}
		}

		public static TimeSeriesFrame Get(string datasetId, object request)
		{
			return Get(Client, datasetId, request);
		}

		public static TimeSeriesFrame Get(Client client, string datasetId, object request)
		{
			var dataset = client.GetDataset(datasetId).GetAwaiter().GetResult();

			var requestBuilder = new TimeSeriesRequestBuilder(client, dataset, request.ToPropertySet());
			var dataRequest = requestBuilder.GetRequest();

			var dataBegin = client.GetDataBegin(dataRequest).GetAwaiter().GetResult();
			if (dataBegin == null)
				return null;

			var frameBuilder = new TimeSeriesFrameBuilder(dataset, requestBuilder.DimensionIdsMap, requestBuilder.GetDimensionMembersMapping);
			if (dataBegin.Data != null)
				frameBuilder.AddRange(dataBegin.Data);

			for (var continuationToken = dataBegin.ContinuationToken; !string.IsNullOrEmpty(continuationToken); )
			{
				var dataNext = client.GetDataStreaming(continuationToken).GetAwaiter().GetResult();
				if (dataNext == null)
					break;
				if (dataNext.Data != null)
					frameBuilder.AddRange(dataNext.Data);
				continuationToken = dataNext.ContinuationToken;
			}

			return frameBuilder.GetResult();
		}
	}
}
