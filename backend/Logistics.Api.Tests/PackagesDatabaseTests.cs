
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Logistics.Api.Tests;

public class PackagesDatabaseTests
{
    [Fact]
    public async Task GetPackages_ReturnsList()
    {
        using var factory =
            new IntegrationTestWebApplicationFactory();

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/packages");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var packages = await response.Content
            .ReadFromJsonAsync<List<PackageResponse>>();

        Assert.NotNull(packages);
    }

    [Fact]
    public async Task CreatePackage_ThenGetPackage_ReturnsCreatedPackage()
    {
        using var factory =
            new IntegrationTestWebApplicationFactory();

        using var client = factory.CreateClient();

        // Step 1: Create a package
        var request = new
        {
            customerId = 1,
            destination = "Test City",
            status = "Registered"
        };

        var postResponse = await client.PostAsJsonAsync(
            "/api/packages", request);

        Assert.Equal(HttpStatusCode.Created,
            postResponse.StatusCode);

        var createdPackage = await postResponse.Content
            .ReadFromJsonAsync<PackageResponse>();

        Assert.NotNull(createdPackage);

        int packageId = createdPackage.PackageId;

        try
        {
            // Step 2: Fetch the newly created package
            var getResponse = await client.GetAsync(
                $"/api/packages/{packageId}");

            Assert.Equal(HttpStatusCode.OK,
                getResponse.StatusCode);

            var fetchedPackage = await getResponse.Content
                .ReadFromJsonAsync<PackageResponse>();

            Assert.NotNull(fetchedPackage);

            // Step 3: Verify the data
            Assert.Equal(packageId, fetchedPackage.PackageId);
            Assert.Equal("Test City", fetchedPackage.Destination);
            Assert.Equal("Registered", fetchedPackage.Status);
        }
        finally
        {
            // Step 4: Clean up test data
            await client.DeleteAsync(
                $"/api/packages/{packageId}");
        }
    }


    [Fact]
    public async Task UpdatePackage_ChangesDestinationButNotStatus()
    {
        using var factory =
            new IntegrationTestWebApplicationFactory();

        using var client = factory.CreateClient();

        // Create a package for this test
        var createRequest = new
        {
            customerId = 1,
            destination = "Lund",
            status = "Registered"
        };

        var postResponse = await client.PostAsJsonAsync(
            "/api/packages", createRequest);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content
            .ReadFromJsonAsync<PackageResponse>();

        Assert.NotNull(created);

        int packageId = created.PackageId;

        try
        {
            // Update the destination
            var updateRequest = new
            {
                destination = "Malmö"
            };

            var putResponse = await client.PutAsJsonAsync(
                $"/api/packages/{packageId}", updateRequest);

            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Fetch the package again
            var getResponse = await client.GetAsync(
                $"/api/packages/{packageId}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var updated = await getResponse.Content
                .ReadFromJsonAsync<PackageResponse>();

            Assert.NotNull(updated);
            Assert.Equal("Malmö", updated.Destination);
            Assert.Equal("Registered", updated.Status);
        }
        finally
        {
            // Remove the package created for this test
            await client.DeleteAsync($"/api/packages/{packageId}");
        }
    }


    [Fact]
    public async Task DeletePackage_RemovesPackage()
    {
        using var factory =
            new IntegrationTestWebApplicationFactory();

        using var client = factory.CreateClient();

        // Create a package to delete
        var createRequest = new
        {
            customerId = 1,
            destination = "Test Delete City",
            status = "Registered"
        };

        var postResponse = await client.PostAsJsonAsync(
            "/api/packages", createRequest);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content
            .ReadFromJsonAsync<PackageResponse>();

        Assert.NotNull(created);

        int packageId = created.PackageId;

        try
        {
            // Delete the package
            var deleteResponse = await client.DeleteAsync(
                $"/api/packages/{packageId}");

            Assert.Equal(
                HttpStatusCode.NoContent,
                deleteResponse.StatusCode);

            // Verify that the package no longer exists
            var getResponse = await client.GetAsync(
                $"/api/packages/{packageId}");

            Assert.Equal(
                HttpStatusCode.NotFound,
                getResponse.StatusCode);
        }
        finally
        {
            // Clean up if the test failed before deletion
            await client.DeleteAsync($"/api/packages/{packageId}");
        }
    }



    private class PackageResponse
    {
        public int PackageId { get; set; }
        public string Destination { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
