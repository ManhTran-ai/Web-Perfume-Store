using Microsoft.EntityFrameworkCore;
using GuhaStore.Core.Entities;

namespace GuhaStore.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Capacity> Capacities { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Evaluate> Evaluates { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryDetail> InventoryDetails { get; set; }
    public DbSet<Metric> Metrics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Account entity mapping
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("account");
            entity.HasKey(e => e.AccountId);
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AccountName).HasColumnName("account_name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.AccountPassword).HasColumnName("account_password").HasMaxLength(100).IsRequired();
            entity.Property(e => e.AccountEmail).HasColumnName("account_email").HasMaxLength(255).IsRequired();
            entity.Property(e => e.AccountPhone).HasColumnName("account_phone").HasMaxLength(20);
            entity.Property(e => e.AccountType).HasColumnName("account_type").IsRequired();
            entity.Property(e => e.AccountStatus).HasColumnName("account_status").IsRequired();
        });

        // Category entity mapping
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("category");
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName).HasColumnName("category_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CategoryDescription).HasColumnName("category_description").HasColumnType("text").IsRequired();
            entity.Property(e => e.CategoryImage).HasColumnName("category_image").HasMaxLength(100).IsRequired();
        });

        // Brand entity mapping
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("brand");
            entity.HasKey(e => e.BrandId);
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.BrandName).HasColumnName("brand_name").HasMaxLength(50).IsRequired();
        });

        // Capacity entity mapping
        modelBuilder.Entity<Capacity>(entity =>
        {
            entity.ToTable("capacity");
            entity.HasKey(e => e.CapacityId);
            entity.Property(e => e.CapacityId).HasColumnName("capacity_id");
            entity.Property(e => e.CapacityName).HasColumnName("capacity_name").HasMaxLength(50).IsRequired();
        });

        // Collection entity mapping
        modelBuilder.Entity<Collection>(entity =>
        {
            entity.ToTable("collection");
            entity.HasKey(e => e.CollectionId);
            entity.Property(e => e.CollectionId).HasColumnName("collection_id");
            entity.Property(e => e.CollectionName).HasColumnName("collection_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CollectionKeyword).HasColumnName("collection_keyword").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CollectionImage).HasColumnName("collection_image").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CollectionDescription).HasColumnName("collection_description").HasMaxLength(255).IsRequired();
            entity.Property(e => e.CollectionOrder).HasColumnName("collection_order").IsRequired();
            entity.Property(e => e.CollectionType).HasColumnName("collection_type").IsRequired();
        });

        // Product entity mapping
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName).HasColumnName("product_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.ProductCategory).HasColumnName("product_category").IsRequired();
            entity.Property(e => e.ProductBrand).HasColumnName("product_brand").IsRequired();
            entity.Property(e => e.CapacityId).HasColumnName("capacity_id").IsRequired();
            entity.Property(e => e.ProductQuantity).HasColumnName("product_quantity").IsRequired();
            entity.Property(e => e.QuantitySales).HasColumnName("quantity_sales").IsRequired();
            entity.Property(e => e.ProductPriceImport).HasColumnName("product_price_import").IsRequired();
            entity.Property(e => e.ProductPrice).HasColumnName("product_price").IsRequired();
            entity.Property(e => e.ProductSale).HasColumnName("product_sale").IsRequired();
            entity.Property(e => e.ProductDescription).HasColumnName("product_description").HasColumnType("text").IsRequired();
            entity.Property(e => e.ProductImage).HasColumnName("product_image").HasColumnType("text").IsRequired();
            entity.Property(e => e.ProductStatus).HasColumnName("product_status").IsRequired();

            // Relationships
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.ProductCategory)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.ProductBrand)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Capacity)
                .WithMany()
                .HasForeignKey(p => p.CapacityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProductVariant entity mapping
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.ToTable("product_variants");
            entity.HasKey(e => e.VariantId);
            entity.Property(e => e.VariantId).HasColumnName("variant_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(e => e.CapacityId).HasColumnName("capacity_id").IsRequired();
            entity.Property(e => e.VariantPrice).HasColumnName("variant_price");
            entity.Property(e => e.VariantQuantity).HasColumnName("variant_quantity").IsRequired();
            entity.Property(e => e.VariantStatus).HasColumnName("variant_status").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();

            // Relationships
            entity.HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pv => pv.Capacity)
                .WithMany(c => c.ProductVariants)
                .HasForeignKey(pv => pv.CapacityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint
            entity.HasIndex(e => new { e.ProductId, e.CapacityId }).IsUnique();
        });

        // Order entity mapping
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderCode).HasColumnName("order_code").IsRequired();
            entity.Property(e => e.OrderDate).HasColumnName("order_date").HasMaxLength(50).IsRequired();
            entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired();
            entity.Property(e => e.DeliveryId).HasColumnName("delivery_id").IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount").IsRequired();
            entity.Property(e => e.OrderType).HasColumnName("order_type").IsRequired();
            entity.Property(e => e.OrderStatus).HasColumnName("order_status").IsRequired();

            // Relationships
            entity.HasOne(o => o.Account)
                .WithMany()
                .HasForeignKey(o => o.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Delivery)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DeliveryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // OrderDetail entity mapping
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("order_detail");
            entity.HasKey(e => e.OrderDetailId);
            entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");
            entity.Property(e => e.OrderCode).HasColumnName("order_code").IsRequired();
            entity.Property(e => e.ProductId).HasColumnName("product_id").IsRequired();
            // Note: capacity_id may not exist in order_detail table - make it optional
            entity.Property(e => e.CapacityId).HasColumnName("capacity_id").HasDefaultValue(0);
            entity.Property(e => e.ProductQuantity).HasColumnName("product_quantity").IsRequired();
            entity.Property(e => e.ProductPrice).HasColumnName("product_price").IsRequired();
            entity.Property(e => e.ProductSale).HasColumnName("product_sale").IsRequired();

            // Relationships
            entity.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderCode)
                .HasPrincipalKey(o => o.OrderCode)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(od => od.Capacity)
                .WithMany()
                .HasForeignKey(od => od.CapacityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Customer entity mapping
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customer");
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired();
            entity.Property(e => e.CustomerName).HasColumnName("customer_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CustomerGender).HasColumnName("customer_gender").IsRequired();
            entity.Property(e => e.CustomerEmail).HasColumnName("customer_email").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CustomerPhone).HasColumnName("customer_phone").HasMaxLength(50).IsRequired();
            entity.Property(e => e.CustomerAddress).HasColumnName("customer_address").HasColumnType("text").IsRequired();

            entity.HasOne(c => c.Account)
                .WithMany()
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Delivery entity mapping
        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.ToTable("delivery");
            entity.HasKey(e => e.DeliveryId);
            entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired();
            entity.Property(e => e.DeliveryName).HasColumnName("delivery_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.DeliveryPhone).HasColumnName("delivery_phone").HasMaxLength(20).IsRequired();
            entity.Property(e => e.DeliveryAddress).HasColumnName("delivery_address").HasMaxLength(100).IsRequired();
            entity.Property(e => e.DeliveryNote).HasColumnName("delivery_note").HasMaxLength(100).IsRequired();

            entity.HasOne(d => d.Account)
                .WithMany()
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Article entity mapping
        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable("article");
            entity.HasKey(e => e.ArticleId);
            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.ArticleAuthor).HasColumnName("article_author").HasMaxLength(100).IsRequired();
            entity.Property(e => e.ArticleTitle).HasColumnName("article_title").HasMaxLength(255).IsRequired();
            entity.Property(e => e.ArticleSummary).HasColumnName("article_summary").HasColumnType("text").IsRequired();
            entity.Property(e => e.ArticleContent).HasColumnName("article_content").HasColumnType("text").IsRequired();
            entity.Property(e => e.ArticleImage).HasColumnName("article_image").HasMaxLength(100).IsRequired();
            entity.Property(e => e.ArticleDate).HasColumnName("article_date").IsRequired();
            entity.Property(e => e.ArticleStatus).HasColumnName("article_status").IsRequired();
        });

        // Comment entity mapping
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comment");
            entity.HasKey(e => e.CommentId);
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.ArticleId).HasColumnName("article_id").IsRequired();
            entity.Property(e => e.CommentName).HasColumnName("comment_name").HasMaxLength(50).IsRequired();
            entity.Property(e => e.CommentEmail).HasColumnName("comment_email").HasMaxLength(50).IsRequired();
            entity.Property(e => e.CommentContent).HasColumnName("comment_content").HasColumnType("text").IsRequired();
            entity.Property(e => e.CommentDate).HasColumnName("comment_date").IsRequired();
            entity.Property(e => e.CommentStatus).HasColumnName("comment_status").IsRequired();

            entity.HasOne(c => c.Article)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Evaluate entity mapping
        modelBuilder.Entity<Evaluate>(entity =>
        {
            entity.ToTable("evaluate");
            entity.HasKey(e => e.EvaluateId);
            entity.Property(e => e.EvaluateId).HasColumnName("evaluate_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired();
            entity.Property(e => e.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(e => e.AccountName).HasColumnName("account_name").HasMaxLength(50).IsRequired();
            entity.Property(e => e.EvaluateRate).HasColumnName("evaluate_rate").IsRequired();
            entity.Property(e => e.EvaluateContent).HasColumnName("evaluate_content").HasColumnType("text").IsRequired();
            entity.Property(e => e.EvaluateDate).HasColumnName("evaluate_date").HasMaxLength(50).IsRequired();
            entity.Property(e => e.EvaluateStatus).HasColumnName("evaluate_status").IsRequired();

            entity.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.Evaluates)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Inventory entity mapping
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("inventory");
            entity.HasKey(e => e.InventoryId);
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired();
            entity.Property(e => e.StafName).HasColumnName("staf_name").HasMaxLength(50).IsRequired();
            entity.Property(e => e.SupplierName).HasColumnName("supplier_name").HasMaxLength(50).IsRequired();
            entity.Property(e => e.SupplierPhone).HasColumnName("supplier_phone").HasMaxLength(20).IsRequired();
            entity.Property(e => e.InventoryNote).HasColumnName("inventory_note").HasMaxLength(100).IsRequired();
            entity.Property(e => e.InventoryCode).HasColumnName("inventory_code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.InventoryDate).HasColumnName("inventory_date").IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasColumnType("float").IsRequired();
            entity.Property(e => e.InventoryStatus).HasColumnName("inventory_status").IsRequired();

            entity.HasOne(i => i.Account)
                .WithMany()
                .HasForeignKey(i => i.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // InventoryDetail entity mapping
        modelBuilder.Entity<InventoryDetail>(entity =>
        {
            entity.ToTable("inventory_detail");
            entity.HasKey(e => e.InventoryDetailId);
            entity.Property(e => e.InventoryDetailId).HasColumnName("inventory_detail_id");
            entity.Property(e => e.InventoryCode).HasColumnName("inventory_code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProductId).HasColumnName("product_id").IsRequired();
            entity.Property(e => e.ProductQuantity).HasColumnName("product_quantity").IsRequired();
            entity.Property(e => e.ProductPriceImport).HasColumnName("product_price_import").HasColumnType("float").IsRequired();

            entity.HasOne(id => id.Inventory)
                .WithMany(i => i.InventoryDetails)
                .HasForeignKey(id => id.InventoryCode)
                .HasPrincipalKey(i => i.InventoryCode)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(id => id.Product)
                .WithMany()
                .HasForeignKey(id => id.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Metric entity mapping
        modelBuilder.Entity<Metric>(entity =>
        {
            entity.ToTable("metrics");
            entity.HasKey(e => e.MetricId);
            entity.Property(e => e.MetricId).HasColumnName("metric_id");
            entity.Property(e => e.MetricDate).HasColumnName("metric_date").IsRequired();
            entity.Property(e => e.MetricOrder).HasColumnName("metric_order").IsRequired();
            entity.Property(e => e.MetricSales).HasColumnName("metric_sales").HasMaxLength(100).IsRequired();
            entity.Property(e => e.MetricQuantity).HasColumnName("metric_quantity").IsRequired();
        });
    }
}

