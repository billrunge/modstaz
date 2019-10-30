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
    public static class DeleteStorageArea
    {
        [FunctionName("DeleteStorageArea")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int storageAreaId = data.StorageAreaID;

            Libraries.DeleteStorageArea delete = new Libraries.DeleteStorageArea()
            {
                StorageAreaId = storageAreaId
            };

            await delete.DeleteStorageAreaAsync();

            return (ActionResult)new OkObjectResult($"StorageArea with ID { storageAreaId } deleted successfully");
        }
    }

}
