using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IntegrationApps
{
    public class HttpExample
    {
        private readonly ILogger<HttpExample> _logger;

        public HttpExample(ILogger<HttpExample> logger)
        {
            _logger = logger;
        }

        [Function("HttpExample")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, string? name)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var message = "Welcome to Azure Functions";
            name ??= "!";
            message += $", {name}";
            return new OkObjectResult(message);
        }
    }
}
