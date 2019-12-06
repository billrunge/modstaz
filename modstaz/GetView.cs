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
    public static class GetView
    {
        [FunctionName("GetView")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            int storageAreaId = data.StorageAreaId;
            int viewId = data.ViewId;
            string jwtString = data.JWT;

            JWT jwtHelper = new JWT()
            {
                Key = $"xmRfrELZ#hEZKJEGgeQX9gKAkIMD#%RB5GHG%02lsFonn*^!&&YVDLe7L$*JMf3fgdz&B"
            };

            string jwtResp = jwtHelper.ParseJWT(jwtString);

            if (jwtResp != "false")
            {
                View view = new View() { StorageAreaId = storageAreaId };
                return new OkObjectResult(await view.GetViewColumnsAsync(viewId));
            }
            return (ActionResult)new BadRequestObjectResult("Invalid JWT");
        }
    }
}
