using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbAttribute
{
    public int AttributeId { get; set; }

    public string? Name { get; set; }

    public string? Unit { get; set; }

    public virtual ICollection<TbProductAttribute> TbProductAttributes { get; set; } = new List<TbProductAttribute>();
}
