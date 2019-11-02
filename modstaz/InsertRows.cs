using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using modstaz.Libraries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace modstaz.Functions
{
    public static class InsertRows
    {
        [FunctionName("InsertRows")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int storageAreaId = data["StorageAreaId"];
            JArray fieldsArray = data.FieldsArray;

            Row row = new Row() { StorageAreaId = storageAreaId };

            foreach (JObject fields in fieldsArray)
            {
                await row.InsertRowAsync(fields);
            }
            return new OkObjectResult("Fields inserted successfully.");
        }
    }
}
