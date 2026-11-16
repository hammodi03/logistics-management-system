
using System.ComponentModel.DataAnnotations;

namespace Logistics.Api.DTOs;

public class CreateDeliveryDto
{
    [Range(1, int.MaxValue)]
    public int PackageId { get; set; }

    [Range(1, int.MaxValue)]
    public int DriverId { get; set; }
}
