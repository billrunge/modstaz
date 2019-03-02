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
    public static class CreateColumn
    {
        [FunctionName("CreateColumn")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            int storageAreaId = data.StorageAreaID;
            int columnTypeId = data.ColumnTypeID;
            string displayName = data.DisplayName;

            int columnId = await CreateColumnIdAsync(displayName, storageAreaId, columnTypeId);
            string sqlDataType = await GetSqlDataTypeFromColumnTypeId(columnTypeId);
            await CreateColumnInRowTableAsync(storageAreaId, columnId, sqlDataType);

            return (ActionResult)new OkObjectResult($"Column created successfully");
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        public static async Task<int> CreateColumnIdAsync(string displayName, int storageAreaId, int columnTypeId)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    INSERT INTO [{ storageAreaId }Columns] 
                                ([DisplayName], 
                                 [ColumnType], 
                                 [IsEditable], 
                                 [CreatedOn], 
                                 [LastModified]) 
                    VALUES      (@DisplayName, 
                                 @ColumnType, 
                                 1, 
                                 Getutcdate(), 
                                 Getutcdate());


                    SELECT SCOPE_IDENTITY(); ";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@DisplayName", SqlDbType = SqlDbType.NVarChar, Value = displayName });                                             
                command.Parameters.Add(new SqlParameter { ParameterName = "@ColumnType", SqlDbType = SqlDbType.Int, Value = columnTypeId });
                                             
                return Convert.ToInt32(await command.ExecuteScalarAsync());

            }
        }

        public static async Task CreateColumnInRowTableAsync(int storageAreaId, int columnId, string sqlDataType)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    ALTER TABLE [{ storageAreaId }Rows] 
                      ADD [{columnId}] { sqlDataType }";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }


        public static async Task<string> GetSqlDataTypeFromColumnTypeId(int columnTypeId)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                    SELECT [SqlDataType] 
                    FROM   [ColumnTypes] 
                    WHERE  ID = @ColumnTypeID";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@ColumnTypeID", SqlDbType = SqlDbType.Int, Value = columnTypeId });

                return (string)await command.ExecuteScalarAsync();
            }
        }

    }
}
