using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbShipping
{
    public int ShippingId { get; set; }

    public int? OrderId { get; set; }

    public string? ShipperName { get; set; }

    public string? TrackingNumber { get; set; }

    public decimal? ShippingFee { get; set; }

    public DateOnly? EstimatedDelivery { get; set; }

    public DateOnly? ActualDelivery { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual TbOrder? Order { get; set; }
}
