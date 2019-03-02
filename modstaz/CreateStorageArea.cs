using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace modstaz
{
	public static class CreateStorageArea
	{
		[FunctionName("CreateStorageArea")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject(requestBody);

			string name = data.StorageAreaName;
			int userId = data.UserId;

			int storageAreaId = await CreateStorageAreaIdAsync(name, userId);
			await CreateRowsTableAsync(storageAreaId);
			await CreateColumnsTableAsync(storageAreaId);
			await AddAccessAsync(storageAreaId, userId);

			return (ActionResult)new OkObjectResult($"storage area created");
				//: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
		}

		private static async Task<int> CreateStorageAreaIdAsync(string name, int userId)
		{
			using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") } )
			{
				await connection.OpenAsync();

				string sql = $@"
					INSERT INTO [StorageAreas] 
					([Name], [CreatedBy], [CreatedOn], [LastModified]) 
					OUTPUT INSERTED.ID 
					VALUES (@Name, @UserID, GETUTCDATE(), GETUTCDATE())";

				SqlCommand command = new SqlCommand(sql, connection);

				command.Parameters.Add(new SqlParameter { ParameterName = "@Name", SqlDbType = SqlDbType.NVarChar, Value = name });
				command.Parameters.Add(new SqlParameter { ParameterName = "@UserID", SqlDbType = SqlDbType.Int, Value = userId });

				var results = await command.ExecuteScalarAsync();

				if (!int.TryParse(results.ToString(), out int storageAreaId))
				{
					throw new InvalidCastException("Unable to cast StorageAreaID returned from database to int");
				}
				else
				{
					return storageAreaId;
				}  
			}

		}

		private static async Task CreateRowsTableAsync(int storageAreaId)
		{
			using (SqlConnection connection = new SqlConnection())
			{
				connection.ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
				await connection.OpenAsync();

				string sql = $@"
						SET ANSI_NULLS ON;

						SET QUOTED_IDENTIFIER ON;

						CREATE TABLE [dbo].[{ storageAreaId }Rows](
							[1] [int] IDENTITY(1,1) NOT NULL,
							[2] [datetime] NOT NULL,
							[3] [datetime] NOT NULL,
						PRIMARY KEY CLUSTERED 
						(
							[1] ASC
						)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
						) ON [PRIMARY];";

				SqlCommand command = new SqlCommand(sql, connection);

				await command.ExecuteNonQueryAsync();

			}
		}

		private static async Task CreateColumnsTableAsync(int storageAreaId)
		{
			using (SqlConnection connection = new SqlConnection())
			{
				connection.ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
				connection.Open();

				string sql = $@"
						SET ANSI_NULLS ON;

						SET QUOTED_IDENTIFIER ON;

						CREATE TABLE [dbo].[{ storageAreaId }Columns](
							[ID] [int] IDENTITY(1,1) NOT NULL,
							[DisplayName] [nvarchar](255) NOT NULL,
							[DataType] [nvarchar](50) NOT NULL,
							[CreatedOn] [datetime] NOT NULL,
							[LastModified] [datetime] NOT NULL,
						PRIMARY KEY CLUSTERED 
						(
							[ID] ASC
						)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
						) ON [PRIMARY];";

				SqlCommand command = new SqlCommand(sql, connection);

				await command.ExecuteNonQueryAsync();

				sql = $@"
						INSERT INTO [{ storageAreaId }Columns] (DisplayName, DataType, CreatedOn, LastModified)
						VALUES ('ID', 'int', GETUTCDATE(), GETUTCDATE());

						INSERT INTO [{ storageAreaId }Columns] (DisplayName, DataType, CreatedOn, LastModified)
						VALUES ('Created On', 'datetime', GETUTCDATE(), GETUTCDATE());

						INSERT INTO [{ storageAreaId }Columns] (DisplayName, DataType, CreatedOn, LastModified)
						VALUES ('Last Modified', 'datetime', GETUTCDATE(), GETUTCDATE());";

				command = new SqlCommand(sql, connection);

				await command.ExecuteNonQueryAsync();
			}
		}



		private static async Task AddAccessAsync(int storageAreaId, int userId)
		{
			using (SqlConnection connection = new SqlConnection())
			{
				connection.ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
				await connection.OpenAsync();

				string sql = $@"
					INSERT INTO [StorageAreaAccess] VALUES(@UserID, @StorageAreaID, 1)";

				SqlCommand command = new SqlCommand(sql, connection);

				command.Parameters.Add(new SqlParameter { ParameterName = "@StorageAreaID", SqlDbType = SqlDbType.Int, Value = storageAreaId });
				command.Parameters.Add(new SqlParameter { ParameterName = "@UserID", SqlDbType = SqlDbType.Int, Value = userId });

				await command.ExecuteNonQueryAsync();
			}
		}

	}
}
