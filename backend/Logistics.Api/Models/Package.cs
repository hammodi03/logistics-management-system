using System;
using System.Collections.Generic;

namespace Logistics.Api.Models;

public partial class Package
{
    public int PackageId { get; set; }

    public int CustomerId { get; set; }

    public string Destination { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
