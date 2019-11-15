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
    public static class GenerateJWT
    {
        [FunctionName("GenerateJWT")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string emailAddress = data.EmailAddress;

            User user = new User() { EmailAddress = emailAddress};

            if (await user.DoesUserExistAsync())
            {
                int userId = await user.GetUserIdByEmailAddressAsync();
                JWT jwt = new JWT()
                {
                    UserId = userId,
                    EmailAddress = emailAddress,
                    Key = $"xmRfrELZ#hEZKJEGgeQX9gKAkIMD#%RB5GHG%02lsFonn*^!&&YVDLe7L$*JMf3fgdz&B"
                    //Key = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY")
                };

                string jwtString = await jwt.GenerateJwtAsync();
                return (ActionResult)new OkObjectResult(jwtString);

            }

            return (ActionResult)new OkObjectResult("User does not exist");

        }
    }
}
