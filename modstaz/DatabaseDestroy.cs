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

namespace modstaz.Functions
{
    public static class DatabaseDestroy
    {
        [FunctionName("DatabaseDestroy")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Setup setup = new Setup();
            await setup.DropAllModstazTables();

            return (ActionResult)new OkObjectResult($"Instance Destroyed Successfully.");
                //: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
