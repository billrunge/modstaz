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
    public static class DeleteStorageArea
    {
        [FunctionName("DeleteStorageArea")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int storageAreaId = data.StorageAreaID;

            await DeleteStorageAreaAsync(storageAreaId);

            return (ActionResult)new OkObjectResult($"StorageArea with ID { storageAreaId } deleted successfully");
        }


        private static async Task DeleteStorageAreaAsync(int storageAreaId)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    DROP TABLE [{ storageAreaId }Columns]
                    DROP TABLE [{ storageAreaId }Rows]
                    DELETE FROM StorageAreas WHERE ID = @StorageAreaID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@StorageAreaID", SqlDbType = SqlDbType.Int, Value = storageAreaId });
                await command.ExecuteNonQueryAsync();
            }

        }




    }





}
