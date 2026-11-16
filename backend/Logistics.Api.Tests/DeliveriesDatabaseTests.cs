
using System.Net;
using System.Net.Http.Json;
using Logistics.Api.Data;
using Logistics.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Logistics.Api.Tests;

public class DeliveriesDatabaseTests
{
    [Fact]
    public async Task CreateDelivery_CreatesAssignedEvent()
    {
        using var factory =
            new IntegrationTestWebApplicationFactory();

        using var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<LogisticsDbContext>();

        int? packageId = null;
        int? deliveryId = null;
        int? driverId = null;

        try
        {
            // 1. Create a driver in the test database
            var driver = new Driver
            {
                Name = "Integration Test Driver",
                Phone = Guid.NewGuid().ToString("N")[..20]
            };

            db.Drivers.Add(driver);
            await db.SaveChangesAsync();

            driverId = driver.DriverId;

            // 2. Create a package through the API
            var packageRequest = new
            {
                customerId = 1,
                destination = "Lund",
                status = "Registered"
            };

            var packageResponse = await client.PostAsJsonAsync(
                "/api/packages", packageRequest);

            Assert.Equal(
                HttpStatusCode.Created,
                packageResponse.StatusCode);

            var package = await packageResponse.Content
                .ReadFromJsonAsync<PackageResponse>();

            Assert.NotNull(package);

            packageId = package.PackageId;

            // 3. Assign the package to the driver
            var deliveryRequest = new
            {
                packageId = packageId.Value,
                driverId = driverId.Value
            };

            var deliveryResponse = await client.PostAsJsonAsync(
                "/api/deliveries", deliveryRequest);

            Assert.True(
                deliveryResponse.IsSuccessStatusCode,
                $"Delivery POST returned {deliveryResponse.StatusCode}");

            // 4. Find the created delivery in PostgreSQL
            var delivery = await db.Deliveries
                .AsNoTracking()
                .SingleAsync(d => d.PackageId == packageId.Value);

            deliveryId = delivery.DeliveryId;

            Assert.Equal("Assigned", delivery.DeliveryStatus);

            // 5. Fetch the delivery history through the API
            var eventsResponse = await client.GetAsync(
                $"/api/deliveries/{deliveryId}/events");

            Assert.Equal(
                HttpStatusCode.OK,
                eventsResponse.StatusCode);

            var events = await eventsResponse.Content
                .ReadFromJsonAsync<List<DeliveryEventResponse>>();

            Assert.NotNull(events);
            Assert.Single(events);
            Assert.Equal("Assigned", events[0].Status);
        }
        finally
        {
            // Remove test data in foreign-key order
            if (deliveryId.HasValue)
            {
                await db.DeliveryEvents
                    .Where(e => e.DeliveryId == deliveryId.Value)
                    .ExecuteDeleteAsync();

                await db.Deliveries
                    .Where(d => d.DeliveryId == deliveryId.Value)
                    .ExecuteDeleteAsync();
            }

            if (packageId.HasValue)
            {
                await db.Packages
                    .Where(p => p.PackageId == packageId.Value)
                    .ExecuteDeleteAsync();
            }

            if (driverId.HasValue)
            {
                await db.Drivers
                    .Where(d => d.DriverId == driverId.Value)
                    .ExecuteDeleteAsync();
            }
        }
    }


    [Fact]
    public async Task UpdateDeliveryStatus_UpdatesPackageAndHistory()
    {
        using var factory =
            new IntegrationTestWebApplicationFactory();

        using var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<LogisticsDbContext>();

        int? driverId = null;
        int? packageId = null;
        int? deliveryId = null;

        try
        {
            // 1. Create a test driver
            var driver = new Driver
            {
                Name = "Status Test Driver",
                Phone = Guid.NewGuid().ToString("N")[..20]
            };

            db.Drivers.Add(driver);
            await db.SaveChangesAsync();

            driverId = driver.DriverId;

            // 2. Create a test package using POST
            var packageResponse = await client.PostAsJsonAsync(
                "/api/packages",
                new
                {
                    customerId = 1,
                    destination = "Helsingborg",
                    status = "Registered"
                });

            packageResponse.EnsureSuccessStatusCode();

            var package = await packageResponse.Content
                .ReadFromJsonAsync<PackageResponse>();

            Assert.NotNull(package);

            packageId = package.PackageId;

            // 3. Assign a delivery using POST
            var deliveryResponse = await client.PostAsJsonAsync(
                "/api/deliveries",
                new
                {
                    packageId = packageId.Value,
                    driverId = driverId.Value
                });

            deliveryResponse.EnsureSuccessStatusCode();

            var delivery = await db.Deliveries
                .AsNoTracking()
                .SingleAsync(d => d.PackageId == packageId.Value);

            deliveryId = delivery.DeliveryId;

            // 4. Change Assigned -> In transit
            var transitResponse = await client.PutAsJsonAsync(
                $"/api/deliveries/{deliveryId}/status",
                new { status = "In transit" });

            transitResponse.EnsureSuccessStatusCode();

            // 5. Change In transit -> Delivered
            var deliveredResponse = await client.PutAsJsonAsync(
                $"/api/deliveries/{deliveryId}/status",
                new { status = "Delivered" });

            deliveredResponse.EnsureSuccessStatusCode();

            // 6. Check the delivery status in PostgreSQL
            var updatedDelivery = await db.Deliveries
                .AsNoTracking()
                .SingleAsync(d => d.DeliveryId == deliveryId.Value);

            Assert.Equal(
                "Delivered",
                updatedDelivery.DeliveryStatus);

            // 7. Check the package status through GET
            var getPackageResponse = await client.GetAsync(
                $"/api/packages/{packageId}");

            getPackageResponse.EnsureSuccessStatusCode();

            var updatedPackage = await getPackageResponse.Content
                .ReadFromJsonAsync<PackageStatusResponse>();

            Assert.NotNull(updatedPackage);
            Assert.Equal("Delivered", updatedPackage.Status);

            // 8. Check the delivery history through GET
            var eventsResponse = await client.GetAsync(
                $"/api/deliveries/{deliveryId}/events");

            eventsResponse.EnsureSuccessStatusCode();

            var events = await eventsResponse.Content
                .ReadFromJsonAsync<List<DeliveryEventResponse>>();

            Assert.NotNull(events);

            Assert.Equal(
                new[] { "Assigned", "In transit", "Delivered" },
                events.Select(e => e.Status).ToArray());
        }
        finally
        {
            // Clean up the test data
            if (deliveryId.HasValue)
            {
                await db.DeliveryEvents
                    .Where(e => e.DeliveryId == deliveryId.Value)
                    .ExecuteDeleteAsync();

                await db.Deliveries
                    .Where(d => d.DeliveryId == deliveryId.Value)
                    .ExecuteDeleteAsync();
            }

            if (packageId.HasValue)
            {
                await db.Packages
                    .Where(p => p.PackageId == packageId.Value)
                    .ExecuteDeleteAsync();
            }

            if (driverId.HasValue)
            {
                await db.Drivers
                    .Where(d => d.DriverId == driverId.Value)
                    .ExecuteDeleteAsync();
            }
        }
    }


[Fact]
public async Task InvalidStatusTransition_DoesNotChangeDatabase()
{
    using var factory =
        new IntegrationTestWebApplicationFactory();

    using var client = factory.CreateClient();

    using var scope = factory.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<LogisticsDbContext>();

    int? driverId = null;
    int? packageId = null;
    int? deliveryId = null;

    try
    {
        // 1. Create a test driver
        var driver = new Driver
        {
            Name = "Invalid Status Test Driver",
            Phone = Guid.NewGuid().ToString("N")[..20]
        };

        db.Drivers.Add(driver);
        await db.SaveChangesAsync();

        driverId = driver.DriverId;

        // 2. Create a test package
        var packageResponse = await client.PostAsJsonAsync(
            "/api/packages",
            new
            {
                customerId = 1,
                destination = "Lund",
                status = "Registered"
            });

        packageResponse.EnsureSuccessStatusCode();

        var package = await packageResponse.Content
            .ReadFromJsonAsync<PackageResponse>();

        Assert.NotNull(package);
        packageId = package.PackageId;

        // 3. Assign the package to the driver
        var deliveryResponse = await client.PostAsJsonAsync(
            "/api/deliveries",
            new
            {
                packageId = packageId.Value,
                driverId = driverId.Value
            });

        deliveryResponse.EnsureSuccessStatusCode();

        var delivery = await db.Deliveries
            .AsNoTracking()
            .SingleAsync(d => d.PackageId == packageId.Value);

        deliveryId = delivery.DeliveryId;

        // 4. Attempt an invalid status transition
        var invalidResponse = await client.PutAsJsonAsync(
            $"/api/deliveries/{deliveryId}/status",
            new { status = "Delivered" });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            invalidResponse.StatusCode);

        // 5. Verify the delivery status did not change
        var unchangedDelivery = await db.Deliveries
            .AsNoTracking()
            .SingleAsync(d => d.DeliveryId == deliveryId.Value);

        Assert.Equal(
            "Assigned",
            unchangedDelivery.DeliveryStatus);

        // 6. Verify the package status did not change
        var unchangedPackage = await db.Packages
            .AsNoTracking()
            .SingleAsync(p => p.PackageId == packageId.Value);

        Assert.Equal(
            "Registered",
            unchangedPackage.Status);

        // 7. Verify no extra event was created
        var events = await db.DeliveryEvents
            .AsNoTracking()
            .Where(e => e.DeliveryId == deliveryId.Value)
            .ToListAsync();

        Assert.Single(events);
        Assert.Equal("Assigned", events[0].Status);
    }
    finally
    {
        // Clean up test data
        if (deliveryId.HasValue)
        {
            await db.DeliveryEvents
                .Where(e => e.DeliveryId == deliveryId.Value)
                .ExecuteDeleteAsync();

            await db.Deliveries
                .Where(d => d.DeliveryId == deliveryId.Value)
                .ExecuteDeleteAsync();
        }

        if (packageId.HasValue)
        {
            await db.Packages
                .Where(p => p.PackageId == packageId.Value)
                .ExecuteDeleteAsync();
        }

        if (driverId.HasValue)
        {
            await db.Drivers
                .Where(d => d.DriverId == driverId.Value)
                .ExecuteDeleteAsync();
        }
    }
}



    private class PackageResponse
    {
        public int PackageId { get; set; }
    }

    private class DeliveryEventResponse
    {
        public string Status { get; set; } = "";
    }

    private class PackageStatusResponse
    {
        public string Status { get; set; } = "";
    }
}
