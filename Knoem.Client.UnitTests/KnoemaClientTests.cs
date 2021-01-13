using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Knoema.Data;
using Knoema.Search;
using Knoema.Search.TimeseriesSearch;
using Knoema.Series;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Knoema.UnitTests
{
	[TestClass]
	public class KnoemaClientTests
	{
		[Timeout(20 * 1000)]
		[TestMethod]
		public void SearchTest()
		{
			var instance = new Client("knoema.com");

			var scope = SearchScope.Atlas | SearchScope.NamedEntity | SearchScope.Semantic | SearchScope.Timeseries;
			var searchBeginResponse = instance.Search("north korea natural resources", scope, -1, 3, "en-US").GetAwaiter().GetResult();

			Assert.IsTrue(searchBeginResponse.Items.Any());

			var locations = searchBeginResponse.Items.Select(e => e.Location).Where(e => e != null).ToList();

			var group = new List<TimeSeriesDescriptor>();
			var count = 0;
			do
			{
				var searchContinueRequest = new Request
				{
					Count = 200,
					PrepareFacets = true,
					Locations = locations,
				};

				var searchContinueResponse = instance.SearchTimeseries(searchContinueRequest, "en-US").GetAwaiter().GetResult();

				group = searchContinueResponse.Items.SelectMany(e => e.Items).Where(e => e != null).ToList();
				
				count++;

				locations = searchContinueResponse.Items.Select(e => e.Location).Where(e => e != null).ToList();
			}
			while (group.Any());

			Console.WriteLine(string.Format("Queries count = {0}", count));
		}

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

		[TestMethod]
		public void GetDataBegin()
		{
			var client = new Client("knoema.com");
			var request = new PivotRequest
			{
				Dataset = "fzoaozc",
				Header = new List<PivotRequestItem> { new PivotRequestItem { DimensionId = "Time", UiMode = "AllData" } },
				Stub = new List<PivotRequestItem> {	new PivotRequestItem { DimensionId = "country", Members = new List<object> { 1000010 } } },
				Filter = new List<PivotRequestItem> { new PivotRequestItem { DimensionId = "indicator", Members = new List<object> { 1000000 } } },
				Frequencies = new List<string> { "A" },
				DetailColumns = new[] { "*" }
			};

			var result = client.GetDataBegin(request).GetAwaiter().GetResult();

			Assert.IsNotNull(result.Descriptor);
			Assert.AreEqual(2, result.Descriptor.DetailColumns.Count());
			Assert.AreEqual("EndPeriod", result.Descriptor.DetailColumns.ElementAt(0).Name);

			var detailColumns = result.Descriptor.DetailColumns.Select(c => c.Name).OrderBy(c => c).ToArray();
			var detailValues = result.Data.First().DetailValues;
			var valuesColumns = detailValues.Keys.OrderBy(k => k).ToArray();
			Assert.IsTrue(Enumerable.SequenceEqual(detailColumns, valuesColumns));
			Assert.AreEqual(29, detailValues["EndPeriod"].Count());
			Assert.AreEqual(29, detailValues["Annotation"].Count());
		}

		[TestMethod]
		public void GetDatasetSeriesCount()
		{
			var client = new Client("knoema.com", "fFOqlU", "dpuCSts4xBmSLA");

			var seriesCount = client.GetSeriesCount("ooctknb").GetAwaiter().GetResult();
			Assert.AreEqual(7267, seriesCount);
		}

		[TestMethod]
		public void GetDatasetSettingsColumns()
		{
			var client = new Client("knoema.com", "fFOqlU", "dpuCSts4xBmSLA");

			var dataset = client.GetDataset("ooctknb").GetAwaiter().GetResult();
			Assert.AreEqual(10, dataset.Settings.Columns.Count());
			Assert.AreEqual(1, dataset.Settings.Columns.Count(c => c.Name == "Country" && c.GroupedTo == null && c.Status == Meta.ColumnStatus.Dimension));
			Assert.AreEqual(1, dataset.Settings.Columns.Count(c => c.Name == "ISO" && c.GroupedTo == "Country" && c.Status == Meta.ColumnStatus.Dimension && c.GroupedAs == Meta.ColumnGroupingType.Property));
			Assert.AreEqual(1, dataset.Settings.Columns.Count(c => c.Name == "Subject" && c.GroupedTo == null && c.Status == Meta.ColumnStatus.Dimension));
			Assert.AreEqual(1, dataset.Settings.Columns.Count(c => c.Name == "Notes" && c.GroupedTo == "Subject" && c.Status == Meta.ColumnStatus.Dimension && c.GroupedAs == Meta.ColumnGroupingType.Property));
			Assert.AreEqual(1, dataset.Settings.Columns.Count(c => c.Name == "Date" && c.IdName == "Date" && c.GroupedTo == null && c.Status == Meta.ColumnStatus.Date && c.Type == Meta.ColumnType.Date));
			Assert.AreEqual(1, dataset.Settings.Columns.Count(c => c.Name == "Value" && c.IdName == "Value" && c.GroupedTo == null && c.Status == Meta.ColumnStatus.Measure && c.Type == Meta.ColumnType.Number));
			Assert.AreEqual(1, dataset.Settings.Columns.Count(c => c.Name == "Estimates Start After" && c.IdName == "Estimates-Start-After" && c.GroupedTo == null && c.Status == Meta.ColumnStatus.Detail && c.Type == Meta.ColumnType.Text));
		}

		[TestMethod]
		public void SetDatasetReplacement()
		{
			var client = new Client("knoema.com", "fFOqlU", "dpuCSts4xBmSLA");

			client.CreateReplacement("ooctknb", "lhbcznd").GetAwaiter().GetResult();

			var dataset = client.GetDataset("ooctknb").GetAwaiter().GetResult();
			Assert.AreEqual("lhbcznd", dataset.ReplacementDataset.Id);

			client.CreateReplacement("ooctknb", "brzysmc").GetAwaiter().GetResult();

			dataset = client.GetDataset("ooctknb").GetAwaiter().GetResult();
			Assert.AreEqual("brzysmc", dataset.ReplacementDataset.Id);
		}

		[TestMethod]
		[ExpectedException(typeof(WebException))]
		public void SetDatasetReplacementWithError()
		{
			var client = new Client("knoema.com", "fFOqlU", "dpuCSts4xBmSLA");

			client.CreateReplacement("lhbcznd", "brzysmc").GetAwaiter().GetResult();
		}
	}
}
