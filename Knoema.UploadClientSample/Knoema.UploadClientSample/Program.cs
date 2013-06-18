using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
using Knoema.Upload;
using System.IO;

namespace Knoema.UploadClientSample
{
	public class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Syntax:");
				Console.WriteLine("Knoema.UploadClientSample.exe [filename] [datasetName]");
				Console.WriteLine();
				Console.WriteLine("filename - complete file path");
				Console.ReadKey();
				return;
			}
			var uploadTask = Upload(args[0], args[1]);
			uploadTask.Wait();
			if (uploadTask.Result.Status == "Successful")
				Console.WriteLine("The dataset upload is successful and it can be accessed across " + uploadTask.Result.Url);
			else
			{
				Console.WriteLine("The dataset upload for the given file has failed for the following reason: ");
				foreach (var error in uploadTask.Result.Errors)
					Console.WriteLine(error);
			}

			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
		}

		public async static Task<UploadResult> Upload(string filename, string datasetName)
		{
			var result = new UploadResult();

			var client = new Knoema.Client("dev.knoema.org","IaHNjY1F66PKoA","wPWOE/dr0MvL4Da7ufmMGgTCFKw");
			if(File.Exists(filename))
			{
				Console.WriteLine("Uploading dataset ...");
				result = await client.UploadDataset(filename, datasetName);
			}
			else
			{
				result.Status = "Failed";
					result.Errors.Add("File not present in the provided path");
			}
			return result;
		}
	}
}
