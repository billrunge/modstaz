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
    public static class GetStorageAreaColumns
    {
        [FunctionName("GetStorageAreaColumns")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if(!int.TryParse(req.Query["StorageAreaID"], out int storageAreaId))
            {
                throw new InvalidCastException("Unable to cast StorageAreaID from database into an int");
            }

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            return (ActionResult)new OkObjectResult(await GetStorageAreaColumnsAsync(storageAreaId));
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }


        private static async Task<string> GetStorageAreaColumnsAsync(int storageAreaId)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                SELECT [ID], 
                       [DisplayName], 
                       [DataType], 
                       [IsEditable],
                       [CreatedOn], 
                       [LastModified] 
                FROM   [{ storageAreaId }Columns]";

                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    return JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                }

            }

        }

    }
}
