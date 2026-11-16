
using System.ComponentModel.DataAnnotations;

namespace Logistics.Api.DTOs;

public class UpdateDeliveryStatusDto
{
    [Required]
    [RegularExpression("^(Assigned|In transit|Delivered)$",
        ErrorMessage = "Invalid delivery status.")]
    public string Status { get; set; } = string.Empty;
}
