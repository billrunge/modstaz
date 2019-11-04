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
using System.Text.RegularExpressions;

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
            string results = string.Empty;

            foreach (JObject c in columnArray)
            {
                string displayName = (string)c["DisplayName"];
                Regex regex = new Regex("^[0-9]+$");
                if (!regex.IsMatch(displayName))
                {
                    Column column = new Column()
                    {
                        StorageAreaId = storageAreaId,
                        ColumnTypeId = (int)c["ColumnTypeID"],
                        DisplayName = displayName
                    };
                    await column.CreateColumnAsync();
                    results += $"Column '{ displayName }' created successfully." + System.Environment.NewLine;
                } else
                {
                    results += $"Column '{ displayName }' was not created because column names cannot be numbers." + System.Environment.NewLine;
                }
            }

            return (ActionResult)new OkObjectResult(results);
        }
    }
}
