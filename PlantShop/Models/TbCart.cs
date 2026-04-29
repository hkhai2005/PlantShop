using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class TbCart
{
    public int CartId { get; set; }

    public int? CustomerId { get; set; }

    public string? SessionId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual TbCustomer? Customer { get; set; }

    public virtual ICollection<TbCartDetail> TbCartDetails { get; set; } = new List<TbCartDetail>();
}
