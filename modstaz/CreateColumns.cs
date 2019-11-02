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

namespace modstaz
{
    public static class CreateColumn
    {
        [FunctionName("CreateColumns")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            int storageAreaId = data.StorageAreaID;

            JArray columnArray = data.ColumnArray;

            foreach (JObject c in columnArray)
            {
                Column column = new Column()
                {
                    StorageAreaId = storageAreaId,
                    ColumnTypeId = (int)c["ColumnTypeID"],
                    DisplayName = (string)c["DisplayName"]
                };
                await column.CreateColumnAsync();
            }

            return (ActionResult)new OkObjectResult("Columns created successfully.");
        }
    }
}
