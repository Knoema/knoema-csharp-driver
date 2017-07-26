using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Knoema.Data;
using System.Configuration;
using System.Threading.Tasks;

namespace Knoema.ClientSample
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new Knoema.Client(
				ConfigurationManager.AppSettings["host"], ConfigurationManager.AppSettings["clientId"], ConfigurationManager.AppSettings["appSecret"]);

			Console.WriteLine("Getting dataset metadata...");

			client.GetDataset("gquvbhe").ContinueWith(d =>
			{
				Console.WriteLine("Dataset has {0} dimensions: {1}.", d.Result.Dimensions.Count(), string.Join(", ", d.Result.Dimensions.Select(x => x.Name).ToArray()));
			
				var stub = new List<PivotRequestItem>();
				foreach (var dim in d.Result.Dimensions)
				{
					Console.WriteLine(string.Format("Getting \"{0}\" dataset dimension details...", dim.Name));

					client.GetDatasetDimension(d.Result.Id, dim.Id).ContinueWith(t =>
					{
						Console.WriteLine(string.Format("Dimension \"{0}\" has {1} members: {2}.",
							t.Result.Name, t.Result.Items.Count(), string.Join(", ", t.Result.Items.Select(x => x.Name.ToString()).ToArray())));

						stub.Add(new PivotRequestItem()
						{
							DimensionId = t.Result.Id,
							Members = t.Result.Items.Where(x => x.HasData).Select(x => x.Key).Take(10).Cast<object>().ToList()
						});
					}).Wait();					
				}

				var header = new PivotRequestItem() { DimensionId = "Time" };
				for (var i = 1990; i < 2010; i++)
					header.Members.Add(i);

				Console.WriteLine("Getting dataset data...");

				var pivotRequest = new PivotRequest()
				{
					Dataset = "gquvbhe",
					Frequencies = new List<string>() { "A" },
					Stub = stub,
					Header = new List<PivotRequestItem>() { header }
				};

				client.GetData(pivotRequest).ContinueWith(t =>
				{
					t.Result.Tuples.Take(10).ToList().ForEach(x =>
						Console.WriteLine(string.Join(", ", x.Select(y => y.Key + ":" + y.Value).ToList()))
					);

					Console.WriteLine(string.Format("{0} rows of data have been received.", t.Result.Tuples.Count));
					Console.WriteLine("Press any key to exit.");
					Console.ReadKey();
				}).Wait();

			}).Wait();
		}
	}
}
