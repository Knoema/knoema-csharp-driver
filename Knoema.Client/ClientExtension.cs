using Knoema.Data;

using System.Collections.Generic;

namespace Knoema
{
#if NETCOREAPP3_1_OR_GREATER
	public static class ClientExtension
	{
		public static async IAsyncEnumerable<RegularTimeSeriesRawData> GetDataAsync(this Client client, PivotRequest pivot)
		{
			var response = await client.GetDataBegin(pivot);
			foreach (var item in response.Data)
				yield return item;
			while (!string.IsNullOrEmpty(response.ContinuationToken))
			{
				response = await client.GetDataStreaming(response.ContinuationToken);
				foreach (var item in response.Data)
					yield return item;
			}
		}

		public static async IAsyncEnumerable<FlatTimeSeriesRawData> GetFlatDataAsync(this Client client, PivotRequest pivot)
		{
			var response = await client.GetFlatDataBegin(pivot);
			foreach (var item in response.Data)
				yield return item;
			while (!string.IsNullOrEmpty(response.ContinuationToken))
			{
				response = await client.GetFlatDataStreaming(response.ContinuationToken);
				foreach (var item in response.Data)
					yield return item;
			}
		}
	}
#endif
}
