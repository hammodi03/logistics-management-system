
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Logistics.Api.Tests;

public class PackagesApiTests
{
    [Theory]
    [InlineData(1, "Malmö", "Banana")]
    [InlineData(1, "Malmö", "Unknown")]
    [InlineData(1, "", "Registered")]
    [InlineData(0, "Malmö", "Registered")]
    public async Task CreatePackage_InvalidData_ReturnsBadRequest(
        int customerId,
        string destination,
        string status)
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var request = new
        {
            customerId,
            destination,
            status
        };

        var response = await client.PostAsJsonAsync(
            "/api/packages",
            request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}
