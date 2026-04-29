using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbWishlist
{
    public int WishlistId { get; set; }

    public int? CustomerId { get; set; }

    public int? ProductId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual TbCustomer? Customer { get; set; }

    public virtual TbProduct? Product { get; set; }
}
