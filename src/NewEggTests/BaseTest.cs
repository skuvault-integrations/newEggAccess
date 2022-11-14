using CsvHelper;
using CsvHelper.Configuration;
using NewEggAccess.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NewEggTests
{
	public abstract class BaseTest
	{
		protected NewEggConfig Config { get; private set; }
		protected NewEggCredentials Credentials { get; private set; }
		protected abstract string TestSku1 { get; }
		protected abstract string TestSku2 { get; }
		protected abstract string WarehouseLocationCountryCode { get; }
		
		public BaseTest(string fileName)
		{
			var testCredentials = this.LoadTestSettings<TestCredentials>($@"\..\..\{fileName}.csv");
			this.Credentials = new NewEggCredentials(testCredentials.SellerId, testCredentials.ApiKey, testCredentials.SecretKey);
			this.Config = new NewEggConfig((NewEggPlatform)Enum.Parse(typeof(NewEggPlatform), testCredentials.Platform));
		}

		protected T LoadTestSettings<T>(string filePath)
		{
			string basePath = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

			using (var streamReader = new StreamReader(basePath + filePath))
			{
				var csvConfig = new Configuration()
				{
					Delimiter = ","
				};

				using (var csvReader = new CsvReader(streamReader, csvConfig))
				{
					var credentials = csvReader.GetRecords<T>();

					return credentials.FirstOrDefault();
				}
			}
		}
	}
}
