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

    public virtual DbSet<TbBlog> TbBlogs { get; set; }

    public virtual DbSet<TbBlogComment> TbBlogComments { get; set; }

    public virtual DbSet<TbCategory> TbCategories { get; set; }

    public virtual DbSet<TbContact> TbContacts { get; set; }

    public virtual DbSet<TbCustomer> TbCustomers { get; set; }

    public virtual DbSet<TbMenu> TbMenus { get; set; }

    public virtual DbSet<TbOrder> TbOrders { get; set; }

    public virtual DbSet<TbOrderDetail> TbOrderDetails { get; set; }

    public virtual DbSet<TbOrderStatus> TbOrderStatuses { get; set; }

    public virtual DbSet<TbProduct> TbProducts { get; set; }

    public virtual DbSet<TbProductCategory> TbProductCategories { get; set; }

    public virtual DbSet<TbProductReview> TbProductReviews { get; set; }

    public virtual DbSet<TbRole> TbRoles { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("data source=HKhai\\SQLEXPRESS; initial catalog=PlantShopDB; integrated security=True;\nTrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__tb_Accou__349DA5A698F70D2B");

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

        modelBuilder.Entity<TbBlog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__tb_Blog__54379E307B453017");

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
            entity.HasKey(e => e.CommentId).HasName("PK__tb_BlogC__C3B4DFCAC4A9C92F");

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

        modelBuilder.Entity<TbCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__tb_Categ__19093A0B9E5865DE");

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
            entity.HasKey(e => e.ContactId).HasName("PK__tb_Conta__5C66259BE443C54A");

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

        modelBuilder.Entity<TbCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__tb_Custo__A4AE64D8BDD1B923");

            entity.ToTable("tb_Customer");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<TbMenu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__tb_Menu__C99ED23062DF3EF7");

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

        modelBuilder.Entity<TbOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__tb_Order__C3905BCFF310EF7A");

            entity.ToTable("tb_Order");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(150);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.TbOrders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__tb_Order__Custom__59FA5E80");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.TbOrders)
                .HasForeignKey(d => d.OrderStatusId)
                .HasConstraintName("FK__tb_Order__OrderS__5AEE82B9");
        });

        modelBuilder.Entity<TbOrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__tb_Order__D3B9D36CA0F1C50C");

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
            entity.HasKey(e => e.OrderStatusId).HasName("PK__tb_Order__BC674CA18407497E");

            entity.ToTable("tb_OrderStatus");

            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TbProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__tb_Produ__B40CC6CDCE7312BE");

            entity.ToTable("tb_Product");

            entity.Property(e => e.Alias).HasMaxLength(250);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(250);

            entity.HasOne(d => d.CategoryProduct).WithMany(p => p.TbProducts)
                .HasForeignKey(d => d.CategoryProductId)
                .HasConstraintName("FK__tb_Produc__Categ__48CFD27E");
        });

        modelBuilder.Entity<TbProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryProductId).HasName("PK__tb_Produ__FAFA184F6DAD5091");

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

        modelBuilder.Entity<TbProductReview>(entity =>
        {
            entity.HasKey(e => e.ProductReviewId).HasName("PK__tb_Produ__396318807865BC61");

            entity.ToTable("tb_ProductReview");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Detail).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.Product).WithMany(p => p.TbProductReviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__tb_Produc__Produ__4CA06362");
        });

        modelBuilder.Entity<TbRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__tb_Role__8AFACE1AEB2B16E8");

            entity.ToTable("tb_Role");

            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
