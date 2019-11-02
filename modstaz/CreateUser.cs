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
    public static class CreateUser
    {
        [FunctionName("CreateUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            User user = new User()
            {
                EmailAddress = data.EmailAddress
            };

            if (await user.DoesUserExistAsync())
            {
                return (ActionResult)new OkObjectResult($"A user with the email address { data.EmailAddress } already exists.");
            }

            await user.CreateUser();              

            return (ActionResult)new OkObjectResult($"User created successfully.");
        }
    }
}
