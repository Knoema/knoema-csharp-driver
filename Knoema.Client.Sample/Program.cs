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
				Console.Write(
					string.Format("{0}{1}", string.Join(", ", x.Select(y => y.Key + ":" + y.Value).ToList()), Environment.NewLine)
				)
			);

			Console.Write(string.Format("{0} rows of data are recieved. {1}", responseTask.Result.Tuples.Count, Environment.NewLine));
			Console.ReadKey();
		}

		public async Task<PivotResponse> GetData()
		{
			var client = new Knoema.Client(
				ConfigurationManager.AppSettings["host"], ConfigurationManager.AppSettings["appId"], ConfigurationManager.AppSettings["appSecret"]);

			Console.Write(string.Format("Getting dataset metadata... {0}", Environment.NewLine));

			var dataset = await client.GetDataset("gquvbhe");

			Console.Write("Dataset has {0} dimensions: {1}.{2}", 
				dataset.Dimensions.Count(), string.Join(", ", dataset.Dimensions.Select(x => x.Name).ToArray()), Environment.NewLine);

			var stub = new List<PivotRequestItem>();
			foreach (var dim in dataset.Dimensions)
			{
				Console.Write(string.Format("Getting \"{0}\" dataset dimension details... {1}", dim.Name, Environment.NewLine));

				var dimension = await client.GetDatasetDimension(dataset.Id, dim.Id);

				Console.Write(string.Format("Dimension \"{0}\" has {1} members: {2}. {3}",
					dimension.Name, dimension.Items.Count(), string.Join(", ", dimension.Items.Select(x => x.Name.ToString()).ToArray()), Environment.NewLine));

				stub.Add(new PivotRequestItem()
				{
					DimensionId = dimension.Id,
					Members = dimension.Items.Where(x => x.HasData).Select(x => x.Key).Take(10).Cast<object>().ToList()
				});
			}

			var header = new PivotRequestItem() { DimensionId = "Time" };
			for (var i = 1990; i < 2010; i++)
				header.Members.Add(i);

			Console.Write(string.Format("Getting dataset data... {0}", Environment.NewLine));

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
