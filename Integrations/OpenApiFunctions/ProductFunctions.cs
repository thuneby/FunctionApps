using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace OpenApiFunctions
{
    public class ProductFunctions
    {
        private readonly ILogger<ProductFunctions> _logger;
        private readonly ProductRepository _productRepository;

        public ProductFunctions(ProductRepository repository, ILogger<ProductFunctions> log)
        {
            _productRepository = repository;
            _logger = log;
        }

        [FunctionName("GetName")]
        [OpenApiOperation(operationId: "GetName", tags: new[] { "product" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> GetName(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }


        [FunctionName("GetProducts")]
        [OpenApiOperation(operationId: "GetProducts", tags: new[] { "product" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(List<Product>), Description = "The OK response")]
        public async Task<List<Product>> GetProducts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetProducts")] HttpRequest req)
        {
            var result = await _productRepository.GetAll();
            return result.ToList();
        }

        [FunctionName("GetProductById")]
        [OpenApiOperation(operationId: "GetProductById", tags: new[] { "product" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string),
            Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(Product),
            Description = "The OK response")]

        public async Task<Product> GetProductById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetProductById/{id}")] HttpRequest req, string id)
        {

            // Get the product from the database
            var product = await _productRepository.GetById(id);
            return product;
        }


        [FunctionName("AddProduct")]
        [OpenApiOperation(operationId: "AddProduct", tags: new[] { "product" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Product), Description = "Product", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> AddProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "AddProduct")] HttpRequest req)
        {
            var jsonProduct = await req.ReadAsStringAsync();

            try
            {
                var product = JsonConvert.DeserializeObject<Product>(jsonProduct);

                // Add the product to the database
                await _productRepository.Create(product);
                return new OkObjectResult($"Product {product.Name} added successfully");
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to create a product: {jsonProduct}";
                _logger.LogError(errorMessage, ex);
                return new BadRequestObjectResult(errorMessage);
            }
        }

        [FunctionName("UpdateProduct")]
        [OpenApiOperation(operationId: "UpdateProduct", tags: new[] { "product" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Product), Description = "Product",
            Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string),
            Description = "The OK response")]
        public async Task<IActionResult> UpdateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "UpdateProduct")]
            HttpRequest req)
        {
            var jsonProduct = await req.ReadAsStringAsync();

            try
            {
                var product = JsonConvert.DeserializeObject<Product>(jsonProduct);

                // Update the product in the database
                await _productRepository.Update(product);

                return new OkObjectResult($"Product {product.Name} updated successfully");
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to update a product: {jsonProduct}";

                _logger.LogError(errorMessage, ex);
                return new BadRequestObjectResult(errorMessage);
            }
        }


    }
}

