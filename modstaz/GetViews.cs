using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using modstaz.Libraries;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace modstaz
{
    public static class GetStorageAreaViews
    {
        [FunctionName("GetViews")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("GetViews trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string jwtString = data.JWT;
            int storageAreaId = data.StorageAreaId;

            JWT jwtHelper = new JWT()
            {
                Key = $"xmRfrELZ#hEZKJEGgeQX9gKAkIMD#%RB5GHG%02lsFonn*^!&&YVDLe7L$*JMf3fgdz&B"
            };

            string jwtResp = jwtHelper.ParseJWT(jwtString);

            if (jwtResp != "false")
            {
                dynamic jwtData = JsonConvert.DeserializeObject(jwtResp);

                View view = new View() { StorageAreaId = storageAreaId };

                return (ActionResult)new OkObjectResult(await view.GetViewsAsync());
            }

            return new BadRequestObjectResult("Invalid JWT");
        }
    }
}
