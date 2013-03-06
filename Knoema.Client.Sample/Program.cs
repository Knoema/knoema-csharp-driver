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
		static void Main(string[] args)
		{
			var responseTask = new Program().GetData();
			responseTask.Wait();
		
			responseTask.Result.Tuples.Take(10).ToList().ForEach(x =>
				Console.WriteLine(string.Join(", ", x.Select(y => y.Key + ":" + y.Value).ToList()))
			);

			Console.WriteLine(string.Format("{0} rows of data have been received.", responseTask.Result.Tuples.Count));
			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
		}

		public async Task<PivotResponse> GetData()
		{
			var client = new Knoema.Client(
				ConfigurationManager.AppSettings["host"], ConfigurationManager.AppSettings["appId"], ConfigurationManager.AppSettings["appSecret"]);

			Console.WriteLine("Getting dataset metadata...");

			var dataset = await client.GetDataset("gquvbhe");

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

			return await client.GetData(new PivotRequest()
			{
				Dataset = "gquvbhe",
				Frequencies = new List<string>() { "A" },
				Stub = stub,
				Header = new List<PivotRequestItem>() { header }
			});
		}
	}
}
