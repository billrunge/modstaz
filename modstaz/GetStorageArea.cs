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
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if(!int.TryParse(req.Query["StorageAreaID"], out int storageAreaId))
            {
                throw new InvalidCastException("Unable to cast StorageAreaID from database to int");
            }

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            List<KeyValuePair<int, string>> idColumns = await GetColumnsAsync(storageAreaId);
            string result = await GetRowsAsync(storageAreaId, idColumns);


            return (ActionResult)new OkObjectResult(result);
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
            string columnString = $"[{ idColumn[0].Key.ToString() }] AS [{ idColumn[0].Value }]";

            for (int i = 1; i < idColumn.Count; i++)
            {
                columnString = $"[{ idColumn[i].Key.ToString() }] AS [{ idColumn[i].Value }], " + columnString;
            }

            string sql = $@"
                    SELECT { columnString } 
                    FROM   [{ storageAreaId }Rows]";

            return sql;

            //using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            //{

            //}


        }





    }
}
