using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Knoema;
using Knoema.Data;


namespace Knoema.UnitTests
{
	[TestClass]
	public class KnoemaClientTests
	{
		[TestMethod]
		public void GetTimeSeriesList()
		{
			var client = new Client("knoema.com");
			var request = new FullDimensionRequest();
			request.DimensionRequest = new List<DimensionRequestItem>();
			request.DimensionRequest.Add(new DimensionRequestItem()
			{
				DimensionId = "country",
				Members = new List<int>() { 1000000, 1000100 }
			});
			request.DimensionRequest.Add( new DimensionRequestItem()
			{
				DimensionId = "subject",
				Members = new List<int>() { 1000000, 1000200 }
			});
			var tsList = client.GetTimeSeriesList("IMFWEO2017Oct", request).GetAwaiter().GetResult();
			Assert.AreEqual(2, tsList.Count());
			Assert.AreEqual(1062540, tsList.ElementAt(0).TimeseriesKey);
			Assert.AreEqual(1017350, tsList.ElementAt(1).TimeseriesKey);
		}
	}
}
