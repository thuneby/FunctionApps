
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using OpenApiFunctions;

[assembly: FunctionsStartup(typeof(Startup))]

namespace OpenApiFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureServices(builder.Services).BuildServiceProvider(true);
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddScoped<ProductRepository>();
            services.AddDbContext<CosmosContext>(options =>
            {
                options.UseCosmos(
                    accountEndpoint: config["CosmosAccountEndpoint"],
                    accountKey: config["CosmosAccountKey"],
                    databaseName: config["CosmosDatabaseName"]);
            });

            return services;
        }
    }
}
