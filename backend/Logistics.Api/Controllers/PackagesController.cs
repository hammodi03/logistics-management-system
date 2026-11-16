
using Logistics.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Logistics.Api.DTOs;
using Logistics.Api.Models;

namespace Logistics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly LogisticsDbContext _context;

    public PackagesController(LogisticsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPackages()
    {
        var packages = await _context.Packages
            .AsNoTracking()
            .OrderBy(p => p.PackageId)
            .Select(p => new
            {
                p.PackageId,
                p.Destination,
                p.Status
            })
            .ToListAsync();

        return Ok(packages);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPackageById(int id)
    {
        var package = await _context.Packages
            .AsNoTracking()
            .Where(p => p.PackageId == id)
            .Select(p => new
            {
                p.PackageId,
                p.Destination,
                p.Status
            })
            .FirstOrDefaultAsync();

        if (package == null)
        {
            return NotFound();
        }

        return Ok(package);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePackage(
        [FromBody] CreatePackageDto request)
    {
        // Check that the customer exists
        bool customerExists = await _context.Customers
            .AnyAsync(c => c.CustomerId == request.CustomerId);

        if (!customerExists)
        {
            return BadRequest("Customer does not exist.");
        }

        // Create a new package
        var package = new Package
        {
            CustomerId = request.CustomerId,
            Destination = request.Destination,
            Status = request.Status
        };

        // Add the package to the DbContext
        _context.Packages.Add(package);

        // Save changes to PostgreSQL
        await _context.SaveChangesAsync();

        // Return the created package
        return CreatedAtAction(
            nameof(GetPackageById),
            new { id = package.PackageId },
            new
            {
                package.PackageId,
                package.CustomerId,
                package.Destination,
                package.Status
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePackage(
        int id,
        [FromBody] UpdatePackageDto request)
    {
        var package = await _context.Packages
            .FirstOrDefaultAsync(p => p.PackageId == id);

        if (package == null)
        {
            return NotFound();
        }

        package.Destination = request.Destination;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePackage(int id)
    {
        var package = await _context.Packages
            .FirstOrDefaultAsync(p => p.PackageId == id);

        if (package == null)
        {
            return NotFound();
        }

        // Do not delete packages that have deliveries
        bool hasDeliveries = await _context.Deliveries
            .AnyAsync(d => d.PackageId == id);

        if (hasDeliveries)
        {
            return Conflict(
                "Cannot delete a package that has delivery records.");
        }

        _context.Packages.Remove(package);
        await _context.SaveChangesAsync();

        return NoContent();
    }




}
