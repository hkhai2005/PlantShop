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

    public bool? Gender { get; set; }

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? Province { get; set; }

    public bool? VerifyEmail { get; set; }

    public bool? VerifyPhone { get; set; }

    public string? ResetPasswordToken { get; set; }

    public DateTime? TokenExpiry { get; set; }

    public virtual ICollection<TbCart> TbCarts { get; set; } = new List<TbCart>();

    public virtual ICollection<TbNotification> TbNotifications { get; set; } = new List<TbNotification>();

    public virtual ICollection<TbOrder> TbOrders { get; set; } = new List<TbOrder>();

    public virtual ICollection<TbProductReview> TbProductReviews { get; set; } = new List<TbProductReview>();

    public virtual ICollection<TbWishlist> TbWishlists { get; set; } = new List<TbWishlist>();
}
