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

            List<string> inputKeys = fields.Properties().Select(x => x.Name.ToLower()).ToList();

            Column column = new Column() { StorageAreaId = storageAreaId };

            string colsString = await column.GetStorageAreaColumnsAsync();

            JArray columnObj = (JArray)JsonConvert.DeserializeObject(colsString);

            List<string> columnKeys = columnObj.Select(x => x["DisplayName"].ToString().ToLower() ).ToList();

            //List<string> columnNames = columnObj.Properties().Select(x => x.).ToList();


            foreach (string key in columnKeys)
            {
                log.LogInformation(key);
            }

            return new OkObjectResult(fields);

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
