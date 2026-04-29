using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PlantShop.Models;

public partial class PlantShopDbContext : DbContext
{
    public PlantShopDbContext()
    {
    }

    public PlantShopDbContext(DbContextOptions<PlantShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbAccount> TbAccounts { get; set; }

    public virtual DbSet<TbActivityLog> TbActivityLogs { get; set; }

    public virtual DbSet<TbAttribute> TbAttributes { get; set; }

    public virtual DbSet<TbBlog> TbBlogs { get; set; }

    public virtual DbSet<TbBlogComment> TbBlogComments { get; set; }

    public virtual DbSet<TbCart> TbCarts { get; set; }

    public virtual DbSet<TbCartDetail> TbCartDetails { get; set; }

    public virtual DbSet<TbCategory> TbCategories { get; set; }

    public virtual DbSet<TbContact> TbContacts { get; set; }

    public virtual DbSet<TbCoupon> TbCoupons { get; set; }

    public virtual DbSet<TbCustomer> TbCustomers { get; set; }

    public virtual DbSet<TbMenu> TbMenus { get; set; }

    public virtual DbSet<TbNotification> TbNotifications { get; set; }

    public virtual DbSet<TbOrder> TbOrders { get; set; }

    public virtual DbSet<TbOrderCoupon> TbOrderCoupons { get; set; }

    public virtual DbSet<TbOrderDetail> TbOrderDetails { get; set; }

    public virtual DbSet<TbOrderStatus> TbOrderStatuses { get; set; }

    public virtual DbSet<TbProduct> TbProducts { get; set; }

    public virtual DbSet<TbProductAttribute> TbProductAttributes { get; set; }

    public virtual DbSet<TbProductCategory> TbProductCategories { get; set; }

    public virtual DbSet<TbProductImage> TbProductImages { get; set; }

    public virtual DbSet<TbProductReview> TbProductReviews { get; set; }

    public virtual DbSet<TbRole> TbRoles { get; set; }

    public virtual DbSet<TbShipping> TbShippings { get; set; }

    public virtual DbSet<TbWishlist> TbWishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=PlantShopDB;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__tb_Accou__349DA5A695FF7193");

            entity.ToTable("tb_Account");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.TbAccounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__tb_Accoun__RoleI__3A81B327");
        });

        modelBuilder.Entity<TbActivityLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__tb_Activ__5E548648D75C55D5");

            entity.ToTable("tb_ActivityLog");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.UserAgent).HasMaxLength(500);
        });

        modelBuilder.Entity<TbAttribute>(entity =>
        {
            entity.HasKey(e => e.AttributeId).HasName("PK__tb_Attri__C18929EA5E6E0073");

            entity.ToTable("tb_Attribute");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(20);
        });

        modelBuilder.Entity<TbBlog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__tb_Blog__54379E30F5978770");

            entity.ToTable("tb_Blog");

            entity.Property(e => e.Alias).HasMaxLength(250);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SeoDescription).HasMaxLength(500);
            entity.Property(e => e.SeoKeywords).HasMaxLength(250);
            entity.Property(e => e.SeoTitle).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(250);

            entity.HasOne(d => d.Account).WithMany(p => p.TbBlogs)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__tb_Blog__Account__5165187F");

            entity.HasOne(d => d.Category).WithMany(p => p.TbBlogs)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__tb_Blog__Categor__5070F446");
        });

        modelBuilder.Entity<TbBlogComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__tb_BlogC__C3B4DFCAD1CE6232");

            entity.ToTable("tb_BlogComment");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Detail).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.Blog).WithMany(p => p.TbBlogComments)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK__tb_BlogCo__BlogI__5535A963");
        });

        modelBuilder.Entity<TbCart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__tb_Cart__51BCD7B715A23005");

            entity.ToTable("tb_Cart");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SessionId).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithMany(p => p.TbCarts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__tb_Cart__Custome__693CA210");
        });

        modelBuilder.Entity<TbCartDetail>(entity =>
        {
            entity.HasKey(e => e.CartDetailId).HasName("PK__tb_CartD__01B6A6B4899B06CA");

            entity.ToTable("tb_CartDetail");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Cart).WithMany(p => p.TbCartDetails)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__tb_CartDe__CartI__6D0D32F4");

            entity.HasOne(d => d.Product).WithMany(p => p.TbCartDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__tb_CartDe__Produ__6E01572D");
        });

        modelBuilder.Entity<TbCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__tb_Categ__19093A0B2BA0A5BF");

            entity.ToTable("tb_Category");

            entity.Property(e => e.Alias).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.SeoDescription).HasMaxLength(500);
            entity.Property(e => e.SeoKeywords).HasMaxLength(250);
            entity.Property(e => e.SeoTitle).HasMaxLength(250);
            entity.Property(e => e.Title).HasMaxLength(150);
        });

        modelBuilder.Entity<TbContact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__tb_Conta__5C66259B592E0D6B");

            entity.ToTable("tb_Contact");

            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<TbCoupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PK__tb_Coupo__384AF1BA3EA56D7D");

            entity.ToTable("tb_Coupon");

            entity.HasIndex(e => e.Code, "UQ__tb_Coupo__A25C5AA7AD81B759").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountType).HasMaxLength(20);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaxDiscount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinOrderAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UsedCount).HasDefaultValue(0);
        });

        modelBuilder.Entity<TbCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__tb_Custo__A4AE64D89ED9B671");

            entity.ToTable("tb_Customer");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Province).HasMaxLength(100);
            entity.Property(e => e.ResetPasswordToken).HasMaxLength(100);
            entity.Property(e => e.TokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.VerifyEmail).HasDefaultValue(false);
            entity.Property(e => e.VerifyPhone).HasDefaultValue(false);
            entity.Property(e => e.Ward).HasMaxLength(100);
        });

        modelBuilder.Entity<TbMenu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__tb_Menu__C99ED230DE895503");

            entity.ToTable("tb_Menu");

            entity.Property(e => e.Alias).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(150);
        });

        modelBuilder.Entity<TbNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__tb_Notif__20CF2E1295CEB360");

            entity.ToTable("tb_Notification");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Link).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Customer).WithMany(p => p.TbNotifications)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__tb_Notifi__Custo__160F4887");
        });

        modelBuilder.Entity<TbOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__tb_Order__C3905BCF5C69684B");

            entity.ToTable("tb_Order");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CancelReason).HasMaxLength(500);
            entity.Property(e => e.CanceledDate).HasColumnType("datetime");
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(150);
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Unpaid");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ShippingFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.TbOrders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__tb_Order__Custom__59FA5E80");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.TbOrders)
                .HasForeignKey(d => d.OrderStatusId)
                .HasConstraintName("FK__tb_Order__OrderS__5AEE82B9");
        });

        modelBuilder.Entity<TbOrderCoupon>(entity =>
        {
            entity.HasKey(e => e.OrderCouponId).HasName("PK__tb_Order__F0BED53526FB207C");

            entity.ToTable("tb_OrderCoupon");

            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Coupon).WithMany(p => p.TbOrderCoupons)
                .HasForeignKey(d => d.CouponId)
                .HasConstraintName("FK__tb_OrderC__Coupo__04E4BC85");

            entity.HasOne(d => d.Order).WithMany(p => p.TbOrderCoupons)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__tb_OrderC__Order__03F0984C");
        });

        modelBuilder.Entity<TbOrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__tb_Order__D3B9D36C0781CE48");

            entity.ToTable("tb_OrderDetail");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.TbOrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__tb_OrderD__Order__5DCAEF64");

            entity.HasOne(d => d.Product).WithMany(p => p.TbOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__tb_OrderD__Produ__5EBF139D");
        });

        modelBuilder.Entity<TbOrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("PK__tb_Order__BC674CA1C639E65F");

            entity.ToTable("tb_OrderStatus");

            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TbProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__tb_Produ__B40CC6CDDAA642A6");

            entity.ToTable("tb_Product");

            entity.HasIndex(e => e.Sku, "UQ__tb_Produ__CA1ECF0D93BEE6F8").IsUnique();

            entity.Property(e => e.Alias).HasMaxLength(250);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Dimensions).HasMaxLength(100);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.SoldCount).HasDefaultValue(0);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.Views).HasDefaultValue(0);
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CategoryProduct).WithMany(p => p.TbProducts)
                .HasForeignKey(d => d.CategoryProductId)
                .HasConstraintName("FK__tb_Produc__Categ__48CFD27E");
        });

        modelBuilder.Entity<TbProductAttribute>(entity =>
        {
            entity.HasKey(e => e.ProductAttributeId).HasName("PK__tb_Produ__00CE6747E9BFFFB2");

            entity.ToTable("tb_ProductAttribute");

            entity.Property(e => e.Value).HasMaxLength(200);

            entity.HasOne(d => d.Attribute).WithMany(p => p.TbProductAttributes)
                .HasForeignKey(d => d.AttributeId)
                .HasConstraintName("FK__tb_Produc__Attri__797309D9");

            entity.HasOne(d => d.Product).WithMany(p => p.TbProductAttributes)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__tb_Produc__Produ__787EE5A0");
        });

        modelBuilder.Entity<TbProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryProductId).HasName("PK__tb_Produ__FAFA184FFA9E6833");

            entity.ToTable("tb_ProductCategory");

            entity.Property(e => e.Alias).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(150);
        });

        modelBuilder.Entity<TbProductImage>(entity =>
        {
            entity.HasKey(e => e.ProductImageId).HasName("PK__tb_Produ__07B2B1B80DC340A8");

            entity.ToTable("tb_ProductImage");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.Position).HasDefaultValue(0);

            entity.HasOne(d => d.Product).WithMany(p => p.TbProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__tb_Produc__Produ__73BA3083");
        });

        modelBuilder.Entity<TbProductReview>(entity =>
        {
            entity.HasKey(e => e.ProductReviewId).HasName("PK__tb_Produ__39631880BBDAC502");

            entity.ToTable("tb_ProductReview");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Detail).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Reply).HasMaxLength(500);
            entity.Property(e => e.ReplyDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.TbProductReviews)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__tb_Produc__Custo__22751F6C");

            entity.HasOne(d => d.Product).WithMany(p => p.TbProductReviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__tb_Produc__Produ__4CA06362");
        });

        modelBuilder.Entity<TbRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__tb_Role__8AFACE1A77028C18");

            entity.ToTable("tb_Role");

            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<TbShipping>(entity =>
        {
            entity.HasKey(e => e.ShippingId).HasName("PK__tb_Shipp__5FACD580A3378DBD");

            entity.ToTable("tb_Shipping");

            entity.HasIndex(e => e.OrderId, "UQ__tb_Shipp__C3905BCE83B50B0A").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.ShipperName).HasMaxLength(100);
            entity.Property(e => e.ShippingFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TrackingNumber).HasMaxLength(100);

            entity.HasOne(d => d.Order).WithOne(p => p.TbShipping)
                .HasForeignKey<TbShipping>(d => d.OrderId)
                .HasConstraintName("FK__tb_Shippi__Order__0B91BA14");
        });

        modelBuilder.Entity<TbWishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__tb_Wishl__233189EB6D76756D");

            entity.ToTable("tb_Wishlist");

            entity.HasIndex(e => new { e.CustomerId, e.ProductId }, "UQ_Wishlist").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.TbWishlists)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__tb_Wishli__Custo__10566F31");

            entity.HasOne(d => d.Product).WithMany(p => p.TbWishlists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__tb_Wishli__Produ__114A936A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
