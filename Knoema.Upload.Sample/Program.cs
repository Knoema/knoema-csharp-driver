using System;
using System.IO;
using Knoema.Upload;
using Newtonsoft.Json;

namespace Knoema.Upload.Sample
{
	class Program
	{
		public static string host = "dev.knoema.org";
		public static string appId = "6xS3jF0"; // Replace with your application's id
		public static string appSecret = "acJj0buCHpNj4g"; // Replace with your application's secret

		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				var arg1 = "<pathToFile>";
				var arg2 = "<datasetName>";
				Console.WriteLine("Please use the following format to run tool.");
				Console.WriteLine("Knoema.Upload.Sample.exe {0} {1}", arg1, arg2);
				Console.WriteLine("{0}: The path to data file you wish to upload", arg1);
				Console.WriteLine("{0}: The name you wish to give to the dataset", arg2);
				PauseClose();
				return;
			}

			Upload(args[0], args[1]);
			PauseClose();
			return;
		}

		public static void PauseClose()
		{
			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
		}

		public static void Upload(string fileName, string datasetName)
		{
			if (!File.Exists(fileName))
			{
				Console.WriteLine("Path {0} does not refer to a valid file", fileName);
				return;
			}

			var client = new Knoema.Client(host, appId, appSecret);

			Console.WriteLine("Beginning upload process");

			Console.WriteLine("\nFile upload started");

			var postResult = client.UploadPost(fileName).Result;
			if (postResult.Successful)
				Console.WriteLine("File uploaded successfully");
			else
			{
				Console.WriteLine("Found following error during file upload: {0}", postResult.Error);
				return;
			}

			Console.WriteLine("\nFile verification started");

			var verifyResult = client.UploadVerify(postResult.Properties.Location).Result;
			if (verifyResult.Successful)
				Console.WriteLine("File verified successfully");
			else
			{
				Console.WriteLine("Found following error(s) during file verification: {0}", string.Join(",", verifyResult.ErrorList.ToArray()));
				return;
			}
			
			Console.WriteLine("\nProcessing file for upload");

			var dsUpload = new DatasetUpload()
			{
				Name = datasetName,
				UploadFormatType = verifyResult.UploadFormatType,
				Columns = verifyResult.Columns,
				FlatDSUpdateOptions = verifyResult.FlatDSUpdateOptions,
				FileProperty = postResult.Properties
			};

			if (verifyResult.MetadataDetails != null)
			{
				dsUpload.Description = verifyResult.MetadataDetails.Description;
				dsUpload.Source = verifyResult.MetadataDetails.Source;
				dsUpload.Url = verifyResult.MetadataDetails.DatasetRef;
				dsUpload.PubDate = verifyResult.MetadataDetails.PublicationDate;
				dsUpload.AccessedOn = verifyResult.MetadataDetails.AccessedOn;
			}

			var submitResult = client.UploadSubmit(dsUpload).Result;

			UploadResult uploadResult = null;
			var isPending = false;
			var isProcessing = false;
			while (true)
			{
				uploadResult = client.UploadStatus(submitResult.Id).Result;
				if (string.Equals(uploadResult.Status, "pending"))
				{
					if (isPending)
						Console.Write(".");
					else
					{
						Console.Write("File is queued for processing");
						isPending = true;
					}
					System.Threading.Thread.Sleep(5000); //wait for 5 secs before polling for status again
				}
				else if (string.Equals(uploadResult.Status, "processing"))
				{
					if (isProcessing)
						Console.Write(".");
					else
					{
						Console.Write("\nFile processing has started");
						isProcessing = true;
					}
					System.Threading.Thread.Sleep(5000); //wait for 5 secs before polling for status again
				}
				else
					break;
			}

			if (string.Equals(uploadResult.Status, "successful", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("\nFile uploaded sucessfully");
				Console.WriteLine("You can access it url: {0}", uploadResult.Url);
			}
			else //failed
			{
				Console.WriteLine("\nFound following error(s) while uploading file: {0}", string.Join(",", uploadResult.Errors.ToArray()));
				return;
			}
			
			Console.WriteLine("\nFinished upload process\n");
		}
	}
}
