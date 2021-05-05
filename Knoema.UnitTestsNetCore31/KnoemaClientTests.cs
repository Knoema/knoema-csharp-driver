using Knoema.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Knoema.UnitTestsNetCore31
{
	[TestClass]
	public class KnoemaClientTests
	{
		[TestMethod]
		public async Task GetDataStreamingAsync()
		{
			var instance = new Client("knoema.com");
			var request = new PivotRequest
			{
				Dataset = "WBWDI2019Jan",
				Header = new List<PivotRequestItem>
				{ 
					new PivotRequestItem
					{
						DimensionId = "Time",
						UiMode = "AllData"
					}
				},
				Stub = new List<PivotRequestItem> 
				{ 
					new PivotRequestItem
					{ 
						DimensionId = "country", 
						Members = Enumerable.Range(0, 100).Select(i => 1000000 + i * 10).Cast<object>().ToArray()
					},
					new PivotRequestItem
					{ 
						DimensionId = "series",
						Members = Enumerable.Range(0, 100).Select(i => 1000000 + i * 10).Cast<object>().ToArray()
					}
				},
				Frequencies = new List<string> { "A" },
				DetailColumns = new[] { "*" }
			};

			var result = await instance.GetDataAsync(request).ToListAsync();

			Console.WriteLine(result.Count);
		}
	}
}
