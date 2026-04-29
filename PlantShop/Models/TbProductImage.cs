using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbProductImage
{
    public int ProductImageId { get; set; }

    public int? ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsDefault { get; set; }

    public int? Position { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual TbProduct? Product { get; set; }
}
