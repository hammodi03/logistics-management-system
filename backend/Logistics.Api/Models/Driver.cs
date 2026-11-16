using System;
using System.Collections.Generic;

namespace Logistics.Api.Models;

public partial class Driver
{
    public int DriverId { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
