using AzureFunction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace AzureFunction
{
    public static class JsonFunction
    {
        [FunctionName("json")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string json = req.Query["request"];

                var param = JsonConvert.DeserializeObject<TopModel>(json);

                return new JsonResult(param);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                Console.WriteLine(ex.StackTrace);
                return new ContentResult
                {
                    StatusCode = 500,
                    Content = ex.Message
                };
            }
        }

    }
}
