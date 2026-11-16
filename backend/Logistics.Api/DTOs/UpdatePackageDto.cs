using System.ComponentModel.DataAnnotations;

namespace Logistics.Api.DTOs;

public class UpdatePackageDto
{
    [Required]
    [MaxLength(100)]
    public string Destination { get; set; } = string.Empty;
}