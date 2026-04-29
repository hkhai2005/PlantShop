using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbOrderCoupon
{
    public int OrderCouponId { get; set; }

    public int? OrderId { get; set; }

    public int? CouponId { get; set; }

    public decimal? DiscountAmount { get; set; }

    public virtual TbCoupon? Coupon { get; set; }

    public virtual TbOrder? Order { get; set; }
}
