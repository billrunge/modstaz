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
using modstaz.Libraries;

namespace modstaz
{
    public static class GetStorageAreaColumns
    {
        [FunctionName("GetStorageAreaColumns")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int storageAreaId = data.StorageAreaId;

            Column column = new Column() { StorageAreaId = storageAreaId };

            return (ActionResult)new OkObjectResult(await column.GetStorageAreaColumnsAsync());
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
