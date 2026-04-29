using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbCoupon
{
    public int CouponId { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public decimal? MaxDiscount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsedCount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<TbOrderCoupon> TbOrderCoupons { get; set; } = new List<TbOrderCoupon>();
}
