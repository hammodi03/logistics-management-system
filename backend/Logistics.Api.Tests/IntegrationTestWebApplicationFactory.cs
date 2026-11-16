using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace Logistics.Api.Tests;

public class IntegrationTestWebApplicationFactory
: WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
    IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = config.Build();

            var connectionString = settings.GetConnectionString(
    "IntegrationTestConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
        "Integration test connection is missing.");
            }

            var connection = new NpgsqlConnectionStringBuilder(
    connectionString);

            if (connection.Database != "logistics_integration_test_db")
            {
                throw new InvalidOperationException(
        "Tests must use the integration test database.");
            }

            config.AddInMemoryCollection(
    new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] =
    connectionString
        });
        });
    }
}