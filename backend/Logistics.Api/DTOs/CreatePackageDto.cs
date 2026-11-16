
using System.ComponentModel.DataAnnotations;

namespace Logistics.Api.DTOs;

public class CreatePackageDto
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Destination { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    [RegularExpression("^(Registered|In transit|Delivered)$",
    ErrorMessage = "Invalid package status.")]
    public string Status { get; set; } = string.Empty;
}
