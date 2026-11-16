using System;
using System.Collections.Generic;

namespace Logistics.Api.Models;

public partial class Delivery
{
    public int DeliveryId { get; set; }

    public int PackageId { get; set; }

    public int DriverId { get; set; }

    public string DeliveryStatus { get; set; } = null!;

    public DateTime AssignedAt { get; set; }

    public virtual ICollection<DeliveryEvent> DeliveryEvents { get; set; } = new List<DeliveryEvent>();

    public virtual Driver Driver { get; set; } = null!;

    public virtual Package Package { get; set; } = null!;
}
