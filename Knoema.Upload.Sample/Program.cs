using System;
using System.IO;
using Knoema.Upload;
using Newtonsoft.Json;

namespace Knoema.Upload.Sample
{
	class Program
	{
		public static string host = "knoema.org";
		// Replace with your access token
		public static string token = "AAEAAD956R7mWyYqn7OhDffqKeybVjkGsapEtNi4ip85WWHQG2nTHH2FUlKopmF1M3r3kfZjeSx2IBQDL8XA6jX-Dz2ZD3lc5o_ZL-g_3YIdDsOyFQx1Gaxx4tZZ6zGW5gOx_veaM0Ge7vDZ1cJvKRTTpQqE9NZLbIuRWtCnTGgtPDP8-IO0SXgOnWXmUfdWHyUifaxmzYlvo75mcjgSM2yRKrNrO3ujXFFY8hQ9Tluzo7p6qEw8YZfFXgESdYbJRVQw28VBi5SPKtNqF1tdoAUK8zOjxm9ZENTVFzuLBCDReQb9HtxAFPNN5NdBYEV8OSHONvBVhGc0V2sVUxcsgDAAEzaUAQAAAAEAAAMaWHe75jqqg9fTsP2CN2cNKZXeRCq9zofu0E77fQY6hBakQeojXC_aA0_yuOPERLekx8yvUoTuF9IuXf0J7vSr3cfdtpUCGqRz7n9ECTrpg7F3u0idMXFVqwZ7Nsb9ZRJJchcxQDxQ5azpo4v6Y1g9rVK47Hz1ExIq5EhrtRbcY_of0BcFz2Kf10lQLvS6zq3VXcgTMs1yVAgK-a_LVrr3LrAkZY5MmvaqCvcBGKTzDeaN57HVXRVza1L1H0I69VceODhyIHYxXNOaKlNazpqiDwWTIycXRGp20zxuW7R1ll627ziJkw1WEWJS_z6N9anOY6AJf1z8dMVAJxsU1xCX2v7di_3Ex2VuytyXs8uZFyANU3P0cUzzRKLbPFbiYRRAhHXN7O3riMxKF2u7E-TUo4yo7tFGUxkPQ2-kzDnBli1TxJVI4BSu3AFAtL4xE_e0YbC76Vc4BvRkZHW4Lz7R4xx-tVFiPtAgxQCw9foJYiAcAtpLsgqls3ffyDlL-JySeTN_h7kFY_uWbUvCeH0"; 
		/**********************************/
		//ADD THESE IF NO ACCESS TOKEN GIVEN
		/**********************************/
		//you can use application Id, application secret instead of access token
		//if access token is added above then no need to mention below two, make them null
		public static string appId = "1BTrVoo"; // Replace with your application's id
		public static string appSecret = "UF1P9UdwduFKbQ"; // Replace with your application's secret

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

			var client = string.IsNullOrEmpty(token) ? new Knoema.Client(host, appId, appSecret) : new Knoema.Client(host, token);

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
