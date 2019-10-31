using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace modstaz
{
    public static class GetStorageAreas
    {
        [FunctionName("GetStorageAreas")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("GetStorageAreas trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            int userId = data.UserId;

            Libraries.GetStorageAreas getStorageAreas = new Libraries.GetStorageAreas() { UserId = userId };
            
            return new OkObjectResult(await getStorageAreas.GetStorsageAreasAsync());

        }
    }
}
