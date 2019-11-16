using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using modstaz.Libraries;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace modstaz
{
    public static class IsJWTValid
    {
        [FunctionName("GetStorageArea")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int storageAreaId = data.StorageAreaId;

            StorageArea storageArea = new StorageArea() { StorageAreaId = storageAreaId };

            string result = await storageArea.GetStorageAreaAsync();

            return new OkObjectResult(result);
            //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
