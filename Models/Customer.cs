using System;
using System.Collections.Generic;

namespace EF.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? Discount { get; set; }

    public int? Activion { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
