using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using Knoema.Data;
using Knoema.Series;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
			request.DimensionRequest.Add(new DimensionRequestItem
			{
				DimensionId = "country",
				Members = { 1000000, 1000100 }
			});
			request.DimensionRequest.Add(new DimensionRequestItem
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
			var unit = new UnitMember
			{
				Key = 1000230,
				Name = "Percent of potential GDP"
			};
			Assert.AreEqual("Unit(s)", unitsList.ElementAt(0).Name);
			var memberKey = unitsList.FirstOrDefault(m => m.Name == unit.Name).Key;
			Assert.AreEqual(unit.Key, memberKey);
		}

		[TestMethod]
		public void GetTimeseriesData()
		{
			var data = Knoema.Get("IMFWEO2017Oct", new PropSet
			{
				{ "frequency", "A" },
				{ "country", "612;614" },
				{ "subject", "NGDPD;NGDP" }
			});
			Assert.IsNotNull(data);

			data = Knoema.Get("IMFWEO2017Oct", @"
			{
				""frequency"": ""A"",
				""country"": ""612;614"",
				""subject"": ""NGDPD;NGDP""
			}");
			Assert.IsNotNull(data);

			data = Knoema.Get("IMFWEO2017Oct", new
			{
				Frequency = "A",
				TimeRange = "2015-2016",
				Country = "612;614",
				Subject = new[] { "NGDPD", "NGDP" }
			});
			Assert.IsNotNull(data);

			const string countryName = "Angola";
			const string subjectName = "Gross domestic product, current prices (National currency)";

			var series = data[new
			{
				frequency = "A",
				country = countryName,
				subject = subjectName
			}];

			Assert.IsNotNull(series);

			var seriesId = data.MakeId(Frequency.Annual, new[] { 1000030, 1000040 }, new[] { countryName, subjectName });

			Assert.AreEqual(seriesId, series);
			Assert.AreEqual(seriesId.ToString(), series.ToString());

			series = data[new PropSet
			{
				{ "frequency", "A" },
				{ "country", "614" },
				{ "subject", "NGDP" }
			}];

			Assert.IsNotNull(series);

			Assert.AreEqual(seriesId, series);
			Assert.AreEqual(seriesId.ToString(), series.ToString());
		}

		[TestMethod]
		public void UnloadData()
		{
			var clientId = ConfigurationManager.AppSettings["ClientId"];
			if (string.IsNullOrEmpty(clientId))
				return;

			var client = new Client("knoema.org", clientId, "");
			var request = new PivotRequest()
			{
				Dataset = "COMTRADE2015R1",
				Frequencies = new List<string> { "A" }
			};

			request.Header.Add(new PivotRequestItem
			{
				DimensionId = "Time",
				Members = { "2010-2011" },
				UiMode = "range",
			});
			request.Stub.Add(new PivotRequestItem
			{
				DimensionId = "reporter",
				Members = { 1000000, 1000100 }
			});
			request.Stub.Add(new PivotRequestItem
			{
				DimensionId = "indicator",
				Members = { 1000010, 1000020 }
			});
			request.Stub.Add(new PivotRequestItem
			{
				DimensionId = "partner",
				Members = { 1000000, 1000100 }
			});
			request.Stub.Add(new PivotRequestItem
			{
				DimensionId = "commodity",
				Members = { 1000110, 1000100 }
			});

			var tempFolder = Path.GetTempPath() + "KNunload_" + Guid.NewGuid().ToString();
			Directory.CreateDirectory(tempFolder);
			try
			{
				var files = client.UnloadToLocalFolder(request, tempFolder).GetAwaiter().GetResult();
				if (files != null)
				{
					for (var i = 0; i < files.Length; i++)
					{
						var fileName = tempFolder + '\\' + files[i];
						if (File.Exists(fileName))
						{
							try
							{
								File.Delete(fileName);
							}
							catch (Exception)
							{
							}
						}
					}
				}
			}
			finally
			{
				Directory.Delete(tempFolder, recursive: true);
			}
		}
	}
}
