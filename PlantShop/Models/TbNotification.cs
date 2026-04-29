using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbNotification
{
    public int NotificationId { get; set; }

    public int? CustomerId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Link { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual TbCustomer? Customer { get; set; }
}
