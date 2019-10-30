using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace modstaz
{
    public static class GetStorageAreas
    {
        [FunctionName("GetStorageAreas")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("GetStorageAreas trigger function processed a request.");

            //if (!int.TryParse(req.Query["UserId"], out int userId))
            //{
            //    throw new InvalidCastException("Unable to cast UserId to int");
            //}

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            int userId = data.UserId;

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                connection.Open();
                string sql = @"
                        SELECT S.[ID], 
                               S.[Name], 
                               S.[CreatedBy], 
                               S.[CreatedOn], 
                               S.[LastModified] 
                        FROM   [StorageAreas] S 
                               INNER JOIN [StorageAreaAccess] SA 
                                       ON SA.StorageAreaID = S.ID 
                        WHERE  SA.UserID = @UserID";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@UserID", SqlDbType = SqlDbType.Int, Value = userId });

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);

                    return new OkObjectResult(json);
                }
            }
        }
    }
}
