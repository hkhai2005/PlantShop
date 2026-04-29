using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbActivityLog
{
    public int LogId { get; set; }

    public int? AccountId { get; set; }

    public int? CustomerId { get; set; }

    public string? Action { get; set; }

    public string? Ipaddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? CreatedDate { get; set; }
}
