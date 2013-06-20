using System;
using System.IO;
using Knoema.Upload;

namespace Upload.Sample
{
	class Program
	{
		public static string host = "knoema.com";
		public static string appId = "bEkcYAM"; // Replace with your application's id
		public static string appSecret = "WtKKZbOPweAkIw"; // Replace with your application's secret

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

			Console.WriteLine("Beginning upload process...\n");

			Console.WriteLine("Uploading file to server started...");

			var postResult = client.UploadPost(fileName).Result;
			if (postResult.Successful)
				Console.WriteLine("File uploaded successfully...");
			else
			{
				Console.WriteLine("Found following error during file upload: {0}", postResult.Error);
				return;
			}

			Console.WriteLine("File verification started...");

			var verifyResult = client.UploadVerify(postResult.Properties.Location).Result;
			if (verifyResult.Successful)
				Console.WriteLine("File verified successfully...");
			else
			{
				Console.WriteLine("Found following error(s) during file verification: {0}", string.Join(",", verifyResult.ErrorList.ToArray()));
				return;
			}

			Console.WriteLine("Processing file for upload...");

			var submitResult = client.UploadSubmit(new DatasetUpload()
								{
									Name = datasetName,
									UploadFormatType = verifyResult.UploadFormatType,
									Columns = verifyResult.Columns,
									FlatDSUpdateOptions = verifyResult.FlatDSUpdateOptions,
									FileProperty = postResult.Properties
								}).Result;

			UploadResult uploadResult = null; ;
			while (true)
			{
				uploadResult = client.UploadStatus(submitResult.Id).Result;
				if (string.Equals(uploadResult.Status, "in progress", StringComparison.InvariantCultureIgnoreCase))
					System.Threading.Thread.Sleep(5000); //wait for 5 secs before polling for status again
				else
					break;
			}

			if (string.Equals(uploadResult.Status, "successful", StringComparison.InvariantCultureIgnoreCase))
				Console.WriteLine("File is sucessfully uploaded. You can access it url: {0}", uploadResult.Url);
			else //failed
			{
				Console.WriteLine("Found following error(s) while uploading file: {0}", string.Join(",", uploadResult.Errors.ToArray()));
				return;
			}

			Console.WriteLine("Finished upload process...\n");
		}
	}
}
