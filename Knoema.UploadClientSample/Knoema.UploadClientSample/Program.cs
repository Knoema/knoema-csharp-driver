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
			var uploadTask = new Program().upload();
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

		public async Task<UploadResult> upload()
		{
			var result = new UploadResult();

			//var client = new Client(ConfigurationManager.AppSettings["host"], ConfigurationManager.AppSettings["clientId"]);
			var client = new Knoema.Client(
				ConfigurationManager.AppSettings["host"], ConfigurationManager.AppSettings["appId"], ConfigurationManager.AppSettings["appSecret"]);
			var userName = ConfigurationManager.AppSettings["userName"];
			Console.WriteLine("Logging in for user " + userName);
			client.cookies = await client.Login(userName, ConfigurationManager.AppSettings["password"]);

			if (client.cookies == null || client.cookies[".ASPXAUTH"] == null)
			{
				result.Status = "Failed";
				result.Errors.Add("Login failed for user " + userName);
			}
			else
			{
				var filename = ConfigurationManager.AppSettings["filename"];
				if(File.Exists(filename))
				{
					Console.WriteLine("Uploading dataset ...");
					result = await client.UploadDataset(filename, "samplename");
				}
				else
				{
					result.Status = "Failed";
						result.Errors.Add("File not present in the provided path");
				}
			}
			return result;
		}
	}
}
