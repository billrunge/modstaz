using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using modstaz.Libraries;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace modstaz.Functions
{
    public static class InsertRow
    {
        [FunctionName("InsertRow")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int storageAreaId = data["StorageAreaId"];
            JObject fields = data.Fields;

            List<ColumnObject> inputColumns = fields.Properties().Select(x => new ColumnObject { DisplayName = x.Name, Value = (string)x.Value }).ToList();

            Column column = new Column() { StorageAreaId = storageAreaId };

            JArray columnObj = (JArray)JsonConvert.DeserializeObject(await column.GetStorageAreaColumnsAsync());

            List<ColumnObject> columns = columnObj
                .Where(x => (bool)x["IsEditable"] == true)
                .Select(x => new ColumnObject { Id = (int)x["ID"], DisplayName = (string)x["DisplayName"] })
                .ToList();

            List<ColumnObject> updateColumns = (from i in inputColumns
                                               from c in columns.Where(x => i.DisplayName.ToLower() == x.DisplayName.ToLower() || i.DisplayName == x.Id.ToString())                                                               
                                               select new ColumnObject() { Id = c.Id, DisplayName = c.DisplayName, Value = i.Value }).ToList();

            string columnIds = "";
            string values = "";

            foreach (ColumnObject c in updateColumns)
            {
                columnIds += $" [{ c.Id }],";
                values += $"'{ c.Value }',";
            }

            columnIds = columnIds.TrimEnd(',');
            values = values.TrimEnd(',');

            string sql = $@"
                INSERT INTO [{ storageAreaId }ROWS] 
                ( { columnIds } )
                VALUES ( { values } )";

            await InsertRowSqlAsync(sql);

            return new OkObjectResult(sql);

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        static async Task InsertRowSqlAsync(string sql)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();

            }
        }

    }

    class ColumnObject
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
    }
}
