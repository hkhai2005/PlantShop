using System;

namespace PlantShop.Models
{
    public class CartItem
    {
        public TbProduct Product { get; set; } = null!;
        public int Quantity { get; set; }

        // Giá bán thực tế (ưu tiên PriceSale nếu có)
        public decimal Price => (Product.PriceSale ?? Product.Price) ?? 0;

        // Thành tiền
        public decimal Total => Price * Quantity;
    }
}