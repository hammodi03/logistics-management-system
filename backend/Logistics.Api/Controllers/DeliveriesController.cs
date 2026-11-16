
using Logistics.Api.Data;
using Logistics.Api.DTOs;
using Logistics.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Logistics.Api.Services;

namespace Logistics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveriesController : ControllerBase
{
    private readonly LogisticsDbContext _context;

    public DeliveriesController(LogisticsDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDelivery(
        [FromBody] CreateDeliveryDto request)
    {
        // Check if the package exists
        bool packageExists = await _context.Packages
            .AnyAsync(p => p.PackageId == request.PackageId);

        if (!packageExists)
        {
            return BadRequest("Package does not exist.");
        }

        // Check if the driver exists
        bool driverExists = await _context.Drivers
            .AnyAsync(d => d.DriverId == request.DriverId);

        if (!driverExists)
        {
            return BadRequest("Driver does not exist.");
        }

        // Prevent a second active delivery for the same package
        bool activeDeliveryExists = await _context.Deliveries
            .AnyAsync(d =>
                d.PackageId == request.PackageId &&
                d.DeliveryStatus != "Delivered");

        if (activeDeliveryExists)
        {
            return Conflict("Package already has an active delivery.");
        }

        // Create the delivery assignment
        var delivery = new Delivery
        {
            PackageId = request.PackageId,
            DriverId = request.DriverId,
            DeliveryStatus = "Assigned"
        };


        _context.Deliveries.Add(delivery);

        var deliveryEvent = new DeliveryEvent
        {
            Delivery = delivery,
            Status = "Assigned"
        };

        _context.DeliveryEvents.Add(deliveryEvent);

        await _context.SaveChangesAsync();


        return StatusCode(201, new
        {
            delivery.DeliveryId,
            delivery.PackageId,
            delivery.DriverId,
            delivery.DeliveryStatus
        });
    }


    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateDeliveryStatus(
        int id,
        [FromBody] UpdateDeliveryStatusDto request)
    {
        var delivery = await _context.Deliveries
            .Include(d => d.Package)
            .FirstOrDefaultAsync(d => d.DeliveryId == id);

        if (delivery == null)
        {
            return NotFound("Delivery does not exist.");
        }

        string currentStatus = delivery.DeliveryStatus;
        string newStatus = request.Status;

        bool validTransition = DeliveryStatusRules.CanTransition(
            currentStatus,
            newStatus);
        if (!validTransition)
        {
            return Conflict("Invalid delivery status transition.");
        }

        // Update the delivery
        delivery.DeliveryStatus = newStatus;

        // Update the related package
        delivery.Package.Status = newStatus;

        // Record the status change
        var deliveryEvent = new DeliveryEvent
        {
            DeliveryId = delivery.DeliveryId,
            Status = newStatus
        };

        _context.DeliveryEvents.Add(deliveryEvent);

        // Save all changes together
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet("{id:int}/events")]
    public async Task<IActionResult> GetDeliveryEvents(int id)
    {
        // Check if the delivery exists
        bool deliveryExists = await _context.Deliveries
            .AnyAsync(d => d.DeliveryId == id);

        if (!deliveryExists)
        {
            return NotFound("Delivery does not exist.");
        }

        // Get all status events for this delivery
        var events = await _context.DeliveryEvents
            .AsNoTracking()
            .Where(e => e.DeliveryId == id)
            .OrderBy(e => e.EventTime)
            .ThenBy(e => e.EventId)
            .Select(e => new
            {
                e.EventId,
                e.Status,
                e.EventTime
            })
            .ToListAsync();

        return Ok(events);
    }



}
