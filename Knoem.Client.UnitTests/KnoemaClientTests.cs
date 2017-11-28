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
			request.DimensionRequest.Add(new DimensionRequestItem()
			{
				DimensionId = "country",
				Members = { 1000000, 1000100 }
			});
			request.DimensionRequest.Add(new DimensionRequestItem()
			{
				DimensionId = "subject",
				Members = { 1000000, 1000200 }
			});
			var tsList = client.GetTimeSeriesList("IMFWEO2017Oct", request).GetAwaiter().GetResult();
			Assert.AreEqual(2, tsList.Count());
			Assert.AreEqual(1062540, tsList.ElementAt(0).TimeseriesKey);
			Assert.AreEqual(1017350, tsList.ElementAt(1).TimeseriesKey);
		}

		[TestMethod]
		public void GetUnits()
		{
			var client = new Client("knoema.com");
			var unitsList = client.GetUnits().GetAwaiter().GetResult();
			Assert.IsFalse(unitsList.Count() == 0);
			var unit = new UnitMember()
			{
				Key = 1000230,
				Name = "Percent of potential GDP"
			};
			Assert.AreEqual("Unit(s)", unitsList.ElementAt(0).Name);
			var memberKey = unitsList.FirstOrDefault(m => m.Name == unit.Name).Key;
			Assert.AreEqual(unit.Key, memberKey);
		}

	}
}
