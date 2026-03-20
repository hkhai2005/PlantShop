using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbCustomer
{
    public int CustomerId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Avatar { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<TbOrder> TbOrders { get; set; } = new List<TbOrder>();
}
