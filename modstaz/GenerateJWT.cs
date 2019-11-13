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
            int userId = data.UserId;
            string emailAddress = data.EmailAddress;

            JWT jwt = new JWT()
            {
                UserId = userId,
                EmailAddress = emailAddress,
                Key = "&E)H+MbQeThWmZq4t7w!z%C*F-JaNcRfUjXn2r5u8x/A?D(G+KbPeShVkYp3s6v9y$B&E)H@McQfTjWnZq4t7w!z%C*F-JaNdRgUkXp2s5u8x/A?D(G+KbPeShVmYq3t"
            };

            string jwtString = await jwt.GenerateJwtAsync();


            return (ActionResult)new OkObjectResult(jwtString);
        }
    }
}
