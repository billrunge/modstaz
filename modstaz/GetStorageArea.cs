using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace modstaz
{
    public static class GetStorageArea
    {
        [FunctionName("GetStorageArea")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //if (!int.TryParse(req.Query["StorageAreaID"], out int storageAreaId))
            //{
            //    throw new InvalidCastException("Unable to cast StorageAreaID from database to int");
            //}

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int storageAreaId = data.StorageAreaId;

            List<KeyValuePair<int, string>> idColumns = await GetColumnsAsync(storageAreaId);
            string result = await GetRowsAsync(storageAreaId, idColumns);


            return new OkObjectResult(result);
            //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private static async Task<List<KeyValuePair<int, string>>> GetColumnsAsync(int storageAreaId)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    SELECT [ID], 
                           [DisplayName] 
                    FROM   [{ storageAreaId }Columns] ";

                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    List<KeyValuePair<int, string>> idColumnList = new List<KeyValuePair<int, string>>();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (!int.TryParse(row["ID"].ToString(), out int columnId))
                        {
                            throw new InvalidCastException("Unable to cast Column ID returned from Database to int");
                        }

                        KeyValuePair<int, string> idColumnPair = new KeyValuePair<int, string>(columnId, row["DisplayName"].ToString());

                        idColumnList.Add(idColumnPair);
                    }

                    return idColumnList;
                }
            }
        }


        private static async Task<string> GetRowsAsync(int storageAreaId, List<KeyValuePair<int, string>> idColumn)
        {
            string columnString = $"[{ idColumn[idColumn.Count - 1].Key.ToString() }] AS [{ idColumn[idColumn.Count - 1].Value }]";

            for (int i = idColumn.Count - 2; i >= 0; i--)
            {
                columnString = $"[{ idColumn[i].Key.ToString() }] AS [{ idColumn[i].Value }], " + columnString;
            }

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    SELECT { columnString } 
                    FROM   [{ storageAreaId }Rows]";

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
