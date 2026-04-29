using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbProductAttribute
{
    public int ProductAttributeId { get; set; }

    public int? ProductId { get; set; }

    public int? AttributeId { get; set; }

    public string? Value { get; set; }

    public virtual TbAttribute? Attribute { get; set; }

    public virtual TbProduct? Product { get; set; }
}
