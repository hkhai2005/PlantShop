using System.Collections.Generic;

namespace PlantShop.Models
{
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public int TotalQuantity { get; set; }

        public decimal Total => SubTotal + ShippingFee - Discount;
    }
}