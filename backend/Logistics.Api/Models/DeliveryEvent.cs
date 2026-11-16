using System;
using System.Collections.Generic;

namespace Logistics.Api.Models;

public partial class DeliveryEvent
{
    public int EventId { get; set; }

    public int DeliveryId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime EventTime { get; set; }

    public virtual Delivery Delivery { get; set; } = null!;
}
