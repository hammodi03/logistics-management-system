using System;
using System.Collections.Generic;

namespace Logistics.Api.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
}
