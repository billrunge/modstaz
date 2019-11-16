using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using modstaz.Libraries;

namespace modstaz
{
    public static class CreateStorageArea
    {
        [FunctionName("CreateStorageArea")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string name = data.StorageAreaName;

            string jwtString = data.JWT;

            JWT jwtHelper = new JWT()
            {
                Key = $"xmRfrELZ#hEZKJEGgeQX9gKAkIMD#%RB5GHG%02lsFonn*^!&&YVDLe7L$*JMf3fgdz&B"
            };

            string jwtResp = jwtHelper.ParseJWT(jwtString);

            if (jwtResp != "false")
            {
                dynamic jwtData = JsonConvert.DeserializeObject(jwtResp);

                StorageArea storageArea = new StorageArea()
                {
                    StorageAreaName = name,
                    UserId = jwtData.userId
                };

                await storageArea.CreateStorageAreaAsync();
                return (ActionResult)new OkObjectResult($"storage area created");
            }


            return (ActionResult)new BadRequestObjectResult("Invalid JWT");
        }

    }
}
