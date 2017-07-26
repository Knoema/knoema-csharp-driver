using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knoema.Data;

namespace Knoema.ClientSample
{
	class Program
	{
		private static string host = "knoema.com";
		private static string clientId = "OOBFP0U"; // Replace with your application's client id
		private static string clientSecret = "bCAlu8hKP4sxKw"; // Replace with your application's client secret
		
		static void Main(string[] args)
		{			
			var datasetId = "IMFWEO2017Apr";
			GetData(datasetId).Wait();		
		
			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
		}

		static async Task ListDatasets()
		{
			var client = new Knoema.Client(host); // We do not supply app id and secret to fetch public datasets only
			
			Console.WriteLine("Getting dataset list...");
			var datasets = await client.ListDatasets();
			foreach (var ds in datasets)
				Console.WriteLine("{0} - {1}", ds.Id, ds.Name);
		}

		static async Task GetData(string datasetId)
		{
			var client = new Knoema.Client(host, clientId, clientSecret);

			Console.WriteLine("Getting dataset metadata...");

			var dataset = await client.GetDataset(datasetId);

			Console.WriteLine("Dataset has {0} dimensions: {1}.", 
				dataset.Dimensions.Count(), string.Join(", ", dataset.Dimensions.Select(x => x.Name).ToArray()));

			var stub = new List<PivotRequestItem>();
			foreach (var dim in dataset.Dimensions)
			{
				Console.WriteLine(string.Format("Getting \"{0}\" dataset dimension details...", dim.Name));

				var dimension = await client.GetDatasetDimension(dataset.Id, dim.Id);

				Console.WriteLine(string.Format("Dimension \"{0}\" has {1} members: {2}.",
					dimension.Name, dimension.Items.Count(), string.Join(", ", dimension.Items.Select(x => x.Name.ToString()).ToArray())));

				stub.Add(new PivotRequestItem()
				{
					DimensionId = dimension.Id,
					Members = dimension.Items.Where(x => x.HasData).Select(x => x.Key).Take(10).Cast<object>().ToList()
				});
			}

			var header = new PivotRequestItem() { DimensionId = "Time" };
			for (var i = 1990; i < 2010; i++)
				header.Members.Add(i);

			Console.WriteLine("Getting dataset data...");

			var result = await client.GetData(new PivotRequest()
			{
				Dataset = datasetId,
				Frequencies = new List<string>() { "A" },
				Stub = stub,
				Header = new List<PivotRequestItem>() { header }
			});
			result.Tuples.Take(10).ToList().ForEach(x =>
				Console.WriteLine(string.Join(", ", x.Select(y => y.Key + ":" + y.Value).ToList()))
			);

			Console.WriteLine(string.Format("{0} rows of data have been received.", result.Tuples.Count));
		}
	}
}
