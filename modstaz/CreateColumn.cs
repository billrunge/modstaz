using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace modstaz
{
    public static class CreateColumn
    {
        [FunctionName("CreateColumn")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            int storageAreaId = data.StorageAreaID;
            int columnTypeId = data.ColumnTypeID;
            string displayName = data.DisplayName;

            Libraries.CreateColumn createColumn = new Libraries.CreateColumn()
            {
                StorageAreaId = storageAreaId,
                ColumnTypeId = columnTypeId,
                DisplayName = displayName
            };

            await createColumn.CreateColumnAsync();

            return (ActionResult)new OkObjectResult($"Column created successfully");
            //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
