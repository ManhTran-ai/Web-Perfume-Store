<!-- 4a4eb16b-ba21-4c74-a22e-8397185ab671 31740edc-2b09-494c-95c2-5b12323b81bb -->
# GuhaStore Complete Implementation Plan

## Project Overview

This plan implements the complete GuhaStore e-commerce application, migrating from PHP to C# .NET while maintaining all core functionality including product catalog, shopping cart, order management, admin panel, and customer accounts. Payment integration will be limited to Cash on Delivery (COD) only.

## Technology Stack

- **Backend**: ASP.NET Core MVC
- **Database**: MySQL (existing `dbperfume` database)
- **Database Provider**: MySql.EntityFrameworkCore (Official Oracle MySQL provider)
- **ORM**: Entity Framework Core (Code-First approach, mapping to existing schema)
- **Architecture**: Clean Architecture (4-layer structure)
- **Frontend**: Razor Views + JavaScript/jQuery (maintaining similar UI)
- **Authentication**: Custom implementation (matching existing account table structure)
- **Session Management**: ASP.NET Core Session
- **Email**: MailKit or System.Net.Mail
- **PDF Generation**: iTextSharp or QuestPDF
- **Excel Import/Export**: EPPlus or ClosedXML

## Architecture Overview

```
GuhaStore.Core          → Domain entities, interfaces, domain logic
GuhaStore.Application   → Business services, DTOs, application logic
GuhaStore.Infrastructure → EF Core DbContext, repositories, external services
GuhaStore.Web           → Controllers, Views, presentation layer
```

## Database Schema

The existing MySQL database (`dbperfume`) contains 18 tables:

- `account` - User accounts (admin, staff, customers)
- `product` - Product catalog
- `product_variants` - Product variants (capacity/size variations)
- `category` - Product categories
- `brand` - Product brands
- `capacity` - Product capacity/size options
- `article` - Blog/articles
- `comment` - Article comments
- `inventory` - Stock management
- `inventory_detail` - Inventory line items
- `orders` - Customer orders
- `order_detail` - Order line items
- `customer` - Customer information
- `delivery` - Delivery information
- `evaluate` - Product reviews/ratings
- `vnpay` & `momo` - Payment gateway records (deprecated, COD only)
- `collection` - Product collections
- `metrics` - Analytics/metrics

---

## Phase 1: Project Setup & Infrastructure (Week 1-2)

### 1.1 Fix Target Framework Alignment

- Update [GuhaStore.Core/GuhaStore.Core.csproj](GuhaStore.Core/GuhaStore.Core.csproj) to target `net9.0` (currently `net8.0`)
- Ensure all projects target `net9.0` consistently:
  - GuhaStore.Core: `net9.0`
  - GuhaStore.Application: `net9.0`
  - GuhaStore.Infrastructure: `net9.0`
  - GuhaStore.Web: `net9.0`

### 1.2 Add Required NuGet Packages

**GuhaStore.Infrastructure:**

- `MySql.EntityFrameworkCore` (Official Oracle MySQL provider for EF Core)
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.EntityFrameworkCore.Tools`

**GuhaStore.Application:**

- `AutoMapper.Extensions.Microsoft.DependencyInjection` (optional, for DTO mapping)

**GuhaStore.Web:**

- `Microsoft.AspNetCore.Session`
- `MailKit` (for email functionality)

**Installation:**

```bash
dotnet add GuhaStore.Infrastructure package MySql.EntityFrameworkCore
dotnet add GuhaStore.Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add GuhaStore.Infrastructure package Microsoft.EntityFrameworkCore.Tools
```

### 1.3 Database Configuration

- Add MySQL connection string to [GuhaStore.Web/appsettings.json](GuhaStore.Web/appsettings.json):
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=dbperfume;User=root;Password=;CharSet=utf8mb4;"
    },
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Information"
      }
    },
    "AllowedHosts": "*"
  }
  ```


**Connection String Format for MySql.EntityFrameworkCore:**

- Server: MySQL server address (localhost)
- Database: Database name (dbperfume)
- User: MySQL username (root)
- Password: MySQL password (empty in this case)
- CharSet: utf8mb4 for full UTF-8 support

### 1.4 Configure Dependency Injection

Update [GuhaStore.Web/Program.cs](GuhaStore.Web/Program.cs) to register:

- DbContext with MySQL provider
- Repositories and Unit of Work
- Application Services
- Session middleware
- Static files
- Authentication/Authorization (basic setup)

**Key Configuration:**

```csharp
// Add DbContext with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register repositories and services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Add other service registrations...
```

**Key Files to Create:**

- [GuhaStore.Web/appsettings.json](GuhaStore.Web/appsettings.json) - Configuration
- [GuhaStore.Infrastructure/Data/ApplicationDbContext.cs](GuhaStore.Infrastructure/Data/ApplicationDbContext.cs) - Database context
- [GuhaStore.Web/Program.cs](GuhaStore.Web/Program.cs) - Service configuration

---

## Phase 2: Core Layer - Entities & Interfaces (Week 2-3)

### 2.1 Create Entity Classes in GuhaStore.Core/Entities/

Create entities matching the existing MySQL database schema. Map MySQL snake_case column names to C# PascalCase properties:

- [GuhaStore.Core/Entities/Account.cs](GuhaStore.Core/Entities/Account.cs) - User accounts
  - Properties: `AccountId`, `AccountName`, `AccountPassword`, `AccountEmail`, `AccountPhone`, `AccountType`, `AccountStatus`
  - Map: `account_id` → `AccountId`, `account_name` → `AccountName`, etc.

- [GuhaStore.Core/Entities/Product.cs](GuhaStore.Core/Entities/Product.cs) - Products
  - Properties: `ProductId`, `ProductName`, `CategoryId`, `BrandId`, `CollectionId`, `CapacityId`, `ProductPrice`, `ProductSalePrice`, etc.
  - Navigation properties: `Category`, `Brand`, `Collection`, `Capacity`, `ProductVariants`

- [GuhaStore.Core/Entities/ProductVariant.cs](GuhaStore.Core/Entities/ProductVariant.cs) - Product variants
  - Properties: `ProductVariantId`, `ProductId`, `CapacityId`, `Price`, `SalePrice`, `Stock`, etc.

- [GuhaStore.Core/Entities/Category.cs](GuhaStore.Core/Entities/Category.cs) - Categories
- [GuhaStore.Core/Entities/Brand.cs](GuhaStore.Core/Entities/Brand.cs) - Brands
- [GuhaStore.Core/Entities/Capacity.cs](GuhaStore.Core/Entities/Capacity.cs) - Capacity options
- [GuhaStore.Core/Entities/Collection.cs](GuhaStore.Core/Entities/Collection.cs) - Collections
- [GuhaStore.Core/Entities/Order.cs](GuhaStore.Core/Entities/Order.cs) - Orders
- [GuhaStore.Core/Entities/OrderDetail.cs](GuhaStore.Core/Entities/OrderDetail.cs) - Order details
- [GuhaStore.Core/Entities/Customer.cs](GuhaStore.Core/Entities/Customer.cs) - Customers
- [GuhaStore.Core/Entities/Delivery.cs](GuhaStore.Core/Entities/Delivery.cs) - Delivery info
- [GuhaStore.Core/Entities/Article.cs](GuhaStore.Core/Entities/Article.cs) - Blog articles
- [GuhaStore.Core/Entities/Comment.cs](GuhaStore.Core/Entities/Comment.cs) - Comments
- [GuhaStore.Core/Entities/Evaluate.cs](GuhaStore.Core/Entities/Evaluate.cs) - Product reviews
- [GuhaStore.Core/Entities/Inventory.cs](GuhaStore.Core/Entities/Inventory.cs) - Inventory records
- [GuhaStore.Core/Entities/InventoryDetail.cs](GuhaStore.Core/Entities/InventoryDetail.cs) - Inventory details

**MySQL Data Type Mapping:**

- `INT` → `int` or `int?`
- `VARCHAR` → `string`
- `TEXT` → `string`
- `DECIMAL` → `decimal` or `decimal?`
- `DATETIME` → `DateTime` or `DateTime?`
- `TINYINT(1)` → `bool` or `bool?`
- `BIGINT` → `long` or `long?`

### 2.2 Create Interfaces in GuhaStore.Core/Interfaces/

- [GuhaStore.Core/Interfaces/IRepository.cs](GuhaStore.Core/Interfaces/IRepository.cs) - Generic repository interface
  ```csharp
  public interface IRepository<T> where T : class
  {
      Task<T> GetByIdAsync(int id);
      Task<IEnumerable<T>> GetAllAsync();
      Task<T> AddAsync(T entity);
      Task UpdateAsync(T entity);
      Task DeleteAsync(T entity);
  }
  ```

- [GuhaStore.Core/Interfaces/IUnitOfWork.cs](GuhaStore.Core/Interfaces/IUnitOfWork.cs) - Unit of Work pattern
- [GuhaStore.Core/Interfaces/IProductService.cs](GuhaStore.Core/Interfaces/IProductService.cs) - Product service interface
- [GuhaStore.Core/Interfaces/IArticleService.cs](GuhaStore.Core/Interfaces/IArticleService.cs) - Article service interface
- [GuhaStore.Core/Interfaces/ICartService.cs](GuhaStore.Core/Interfaces/ICartService.cs) - Cart service interface
- [GuhaStore.Core/Interfaces/IInventoryService.cs](GuhaStore.Core/Interfaces/IInventoryService.cs) - Inventory service interface
- [GuhaStore.Core/Interfaces/IFileUploadService.cs](GuhaStore.Core/Interfaces/IFileUploadService.cs) - File upload service interface
- [GuhaStore.Core/Interfaces/IOrderService.cs](GuhaStore.Core/Interfaces/IOrderService.cs) - Order service interface
- [GuhaStore.Core/Interfaces/IEmailService.cs](GuhaStore.Core/Interfaces/IEmailService.cs) - Email service interface
- [GuhaStore.Core/Interfaces/IAnalyticsService.cs](GuhaStore.Core/Interfaces/IAnalyticsService.cs) - Analytics service interface

---

## Phase 3: Infrastructure Layer - Data Access (Week 3-4)

### 3.1 Create DbContext

- [GuhaStore.Infrastructure/Data/ApplicationDbContext.cs](GuhaStore.Infrastructure/Data/ApplicationDbContext.cs)
  - Configure all DbSets for each entity
  - Configure entity relationships (foreign keys, navigation properties)
  - Map MySQL column names to C# properties using Fluent API
  - Configure indexes for performance
  - Handle MySQL-specific data types

**Example Configuration:**

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    // ... other DbSets

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map Account entity
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("account");
            entity.HasKey(e => e.AccountId);
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AccountName).HasColumnName("account_name").HasMaxLength(255);
            entity.Property(e => e.AccountPassword).HasColumnName("account_password").HasMaxLength(100);
            // ... other property mappings
        });

        // Configure relationships
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 3.2 Implement Repository Pattern

- [GuhaStore.Infrastructure/Repositories/Repository.cs](GuhaStore.Infrastructure/Repositories/Repository.cs) - Generic repository implementation
  ```csharp
  public class Repository<T> : IRepository<T> where T : class
  {
      protected readonly ApplicationDbContext _context;
      protected readonly DbSet<T> _dbSet;
  
      public Repository(ApplicationDbContext context)
      {
          _context = context;
          _dbSet = context.Set<T>();
      }
  
      public async Task<T> GetByIdAsync(int id)
      {
          return await _dbSet.FindAsync(id);
      }
  
      // Implement other methods...
  }
  ```

- [GuhaStore.Infrastructure/Repositories/UnitOfWork.cs](GuhaStore.Infrastructure/Repositories/UnitOfWork.cs) - Unit of Work implementation
  - Manages transaction scope
  - Provides access to all repositories
  - Implements `SaveChangesAsync()`

### 3.3 Configure Entity Relationships

- Set up foreign keys and navigation properties
- Configure indexes for performance (product_name, category_id, brand_id, etc.)
- Handle MySQL-specific data types and constraints
- Configure cascade delete behavior where appropriate

---

## Phase 4: Application Layer - Services (Week 4-5)

### 4.1 Implement Services in GuhaStore.Application/Services/

- [GuhaStore.Application/Services/ProductService.cs](GuhaStore.Application/Services/ProductService.cs)
  - `GetActiveProductsAsync()` - Get active products
  - `GetProductByIdAsync(int id)` - Get product by ID with variants
  - `GetProductsByCategoryAsync(int categoryId)` - Filter by category
  - `GetProductsByBrandAsync(int brandId)` - Filter by brand
  - `SearchProductsAsync(string query)` - Search functionality
  - `GetFeaturedProductsAsync()` - Featured products for homepage
  - `GetRelatedProductsAsync(int productId)` - Related products

- [GuhaStore.Application/Services/ArticleService.cs](GuhaStore.Application/Services/ArticleService.cs)
  - `GetAllArticlesAsync()` - Get all articles
  - `GetArticleByIdAsync(int id)` - Get article with comments
  - `GetActiveArticlesAsync()` - Get published articles
  - `GetRecentArticlesAsync(int count)` - Recent articles
  - `CreateArticleAsync(Article article)` - Create article
  - `UpdateArticleAsync(Article article)` - Update article
  - `DeleteArticleAsync(int id)` - Delete article
  - `GetArticleCommentsAsync(int articleId)` - Get comments
  - `AddCommentAsync(Comment comment)` - Add comment

- [GuhaStore.Application/Services/CartService.cs](GuhaStore.Application/Services/CartService.cs)
  - `AddToCartAsync(int productId, int capacityId, int quantity)` - Add to cart (session-based)
  - `UpdateCartItemQuantityAsync(string variantKey, int quantity)` - Update quantity
  - `RemoveFromCartAsync(string variantKey)` - Remove item
  - `GetCartItemsAsync()` - Get cart from session
  - `GetCartTotalAsync()` - Calculate total
  - `GetCartItemCountAsync()` - Get item count
  - `ClearCartAsync()` - Clear cart after order
  - `ValidateCartAsync()` - Validate cart against inventory

- [GuhaStore.Application/Services/InventoryService.cs](GuhaStore.Application/Services/InventoryService.cs)
  - `GetAllInventoriesAsync()` - Get all inventory records
  - `GetInventoryByIdAsync(int id)` - Get inventory by ID
  - `GetInventoriesByStatusAsync(int status)` - Filter by status
  - `CreateInventoryAsync(Inventory inventory)` - Create inventory
  - `UpdateInventoryAsync(Inventory inventory)` - Update inventory
  - `GetLowStockProductsAsync()` - Get low stock alerts

- [GuhaStore.Application/Services/FileUploadService.cs](GuhaStore.Application/Services/FileUploadService.cs)
  - `UploadArticleImageAsync(IFormFile file)` - Upload article image
  - `UploadProductImageAsync(IFormFile file)` - Upload product image
  - `DeleteFileAsync(string filePath)` - Delete file

- [GuhaStore.Application/Services/OrderService.cs](GuhaStore.Application/Services/OrderService.cs)
  - `CreateOrderAsync(Order order, List<OrderDetail> orderDetails)` - Create COD order
  - `GetOrderByIdAsync(int orderId)` - Get order details
  - `GetUserOrdersAsync(int accountId)` - Get user's orders
  - `UpdateOrderStatusAsync(int orderId, int status)` - Update status
  - `ValidateCartInventoryAsync()` - Check stock availability

- [GuhaStore.Application/Services/EmailService.cs](GuhaStore.Application/Services/EmailService.cs)
  - `SendOrderConfirmationAsync(Order order)` - Order confirmation email
  - `SendOrderStatusUpdateAsync(Order order)` - Status update email
  - `SendPasswordResetAsync(string email, string token)` - Password reset

- [GuhaStore.Application/Services/AnalyticsService.cs](GuhaStore.Application/Services/AnalyticsService.cs)
  - `GetSalesMetricsAsync(DateTime startDate, DateTime endDate)` - Sales metrics
  - `GetOrderStatisticsAsync(DateTime startDate, DateTime endDate)` - Order stats
  - `GetTopProductsAsync(int count)` - Top selling products

### 4.2 Create DTOs (if needed)

- [GuhaStore.Application/DTOs/](GuhaStore.Application/DTOs/) - Data transfer objects
  - `ProductDto`, `OrderDto`, `CartItemDto`, etc.
  - Use AutoMapper for entity-to-DTO mapping (optional)

---

## Phase 5: Wire Up Dependency Injection (Week 5)

### 5.1 Update Program.cs

- Register DbContext with MySQL connection:
  ```csharp
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));
  ```

- Register UnitOfWork and Repositories:
  ```csharp
  builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
  ```

- Register all Application Services:
  ```csharp
  builder.Services.AddScoped<IProductService, ProductService>();
  builder.Services.AddScoped<IArticleService, ArticleService>();
  builder.Services.AddScoped<ICartService, CartService>();
  // ... other services
  ```

- Configure Session middleware:
  ```csharp
  builder.Services.AddSession(options =>
  {
      options.IdleTimeout = TimeSpan.FromMinutes(30);
      options.Cookie.HttpOnly = true;
      options.Cookie.IsEssential = true;
  });
  ```

- Add session middleware to pipeline:
  ```csharp
  app.UseSession();
  ```

- Configure static files
- Add authentication/authorization (basic setup)

---

## Phase 6: Authentication & Authorization (Week 6)

### 6.1 Account Management

- [GuhaStore.Web/Controllers/AccountController.cs](GuhaStore.Web/Controllers/AccountController.cs)
  - `Login(string username, string password)` - Login action
  - `Logout()` - Logout action
  - `Register(RegisterViewModel model)` - Register action
  - Password hashing using BCrypt (not MD5 from PHP)
  - Session management for logged-in users

**Password Hashing:**

```csharp
// Hash password (BCrypt)
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

// Verify password
bool isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
```

### 6.2 Authorization Policies

- Configure `StaffOrAdmin` policy in Program.cs:
  ```csharp
  builder.Services.AddAuthorization(options =>
  {
      options.AddPolicy("StaffOrAdmin", policy =>
          policy.RequireAssertion(context =>
              context.User.HasClaim("AccountType", "1") || // Staff
              context.User.HasClaim("AccountType", "2"))); // Admin
  });
  ```

- Map account_type: 0=Customer, 1=Staff, 2=Admin
- Protect admin routes with `[Authorize(Policy = "StaffOrAdmin")]`

### 6.3 Session Configuration

- Configure session in Program.cs (already done in Phase 5)
- Use session for cart storage
- Store user authentication info in session

---

## Phase 7: Order Management - COD Only (Week 7)

### 7.1 Order Service

- [GuhaStore.Application/Services/OrderService.cs](GuhaStore.Application/Services/OrderService.cs)
  - `CreateOrderAsync(Order order, List<OrderDetail> orderDetails)` - Create COD order
  - `GetOrderByIdAsync(int orderId)` - Get order details
  - `GetUserOrdersAsync(int accountId)` - Get user's orders
  - `UpdateOrderStatusAsync(int orderId, int status)` - Update status
  - `ValidateCartInventoryAsync()` - Check stock availability
  - Generate unique order code

### 7.2 Checkout Controller

- [GuhaStore.Web/Controllers/CheckoutController.cs](GuhaStore.Web/Controllers/CheckoutController.cs)
  - `Index()` - Display checkout form with delivery information
  - `ProcessOrder(CheckoutViewModel model)` - Process COD order
    - Validate cart
    - Check inventory
    - Create order and order details
    - Update inventory quantities
    - Clear cart
    - Send confirmation email
  - `OrderConfirmation(int orderId)` - Thank you page

### 7.3 Checkout Views

- [GuhaStore.Web/Views/Checkout/Index.cshtml](GuhaStore.Web/Views/Checkout/Index.cshtml)
  - Delivery information form (name, phone, address)
  - Order summary (cart items, totals)
  - COD payment option only
  - Validation

- [GuhaStore.Web/Views/Checkout/OrderConfirmation.cshtml](GuhaStore.Web/Views/Checkout/OrderConfirmation.cshtml)
  - Order details display
  - Order code
  - Thank you message

---

## Phase 8: Customer Account Management (Week 8)

### 8.1 Account Controller (Customer)

- [GuhaStore.Web/Controllers/AccountController.cs](GuhaStore.Web/Controllers/AccountController.cs)
  - `MyAccount()` - Account dashboard
  - `OrderHistory()` - Display user's orders
  - `OrderDetail(int orderId)` - Order details view
  - `AccountInfo()` - Account information form
  - `UpdateAccountInfo(AccountInfoViewModel model)` - Update account
  - `ChangePassword(ChangePasswordViewModel model)` - Change password

### 8.2 Account Views

- [GuhaStore.Web/Views/Account/MyAccount.cshtml](GuhaStore.Web/Views/Account/MyAccount.cshtml) - Account dashboard
- [GuhaStore.Web/Views/Account/OrderHistory.cshtml](GuhaStore.Web/Views/Account/OrderHistory.cshtml) - Order list
- [GuhaStore.Web/Views/Account/OrderDetail.cshtml](GuhaStore.Web/Views/Account/OrderDetail.cshtml) - Order details
- [GuhaStore.Web/Views/Account/AccountInfo.cshtml](GuhaStore.Web/Views/Account/AccountInfo.cshtml) - Account info form
- [GuhaStore.Web/Views/Account/ChangePassword.cshtml](GuhaStore.Web/Views/Account/ChangePassword.cshtml) - Password change form

---

## Phase 9: Admin Panel - Product Management (Week 9)

### 9.1 Admin Product Controller

- [GuhaStore.Web/Controllers/Admin/ProductController.cs](GuhaStore.Web/Controllers/Admin/ProductController.cs)
  - `Index(int? page)` - Product list with pagination
  - `Create()` - Add new product (GET)
  - `Create(ProductViewModel model)` - Add new product (POST)
  - `Edit(int id)` - Edit product (GET)
  - `Edit(int id, ProductViewModel model)` - Edit product (POST)
  - `Delete(int id)` - Delete product
  - `Search(string query)` - Search products
  - `ManageVariants(int productId)` - Manage product variants
  - `Import()` - Import from Excel
  - `Export()` - Export to Excel

### 9.2 Admin Product Views

- [GuhaStore.Web/Views/Admin/Product/Index.cshtml](GuhaStore.Web/Views/Admin/Product/Index.cshtml) - Product list with pagination
- [GuhaStore.Web/Views/Admin/Product/Create.cshtml](GuhaStore.Web/Views/Admin/Product/Create.cshtml) - Create product form
- [GuhaStore.Web/Views/Admin/Product/Edit.cshtml](GuhaStore.Web/Views/Admin/Product/Edit.cshtml) - Edit product form
- [GuhaStore.Web/Views/Admin/Product/ManageVariants.cshtml](GuhaStore.Web/Views/Admin/Product/ManageVariants.cshtml) - Variant management

### 9.3 File Upload for Product Images

- Implement image upload in ProductController
- Validate file types and sizes
- Store images in `wwwroot/uploads/products/`
- Update product image paths in database

---

## Phase 10: Admin Panel - Order Management (Week 10)

### 10.1 Admin Order Controller

- [GuhaStore.Web/Controllers/Admin/OrderController.cs](GuhaStore.Web/Controllers/Admin/OrderController.cs)
  - `Index(int? status, string search)` - Order list with filtering
  - `Details(int id)` - Order details view
  - `UpdateStatus(int orderId, int status)` - Update order status
  - `Search(string query)` - Search orders
  - `Create()` - Create direct order (if needed)

**Order Status Values:**

- 0: Pending
- 1: Processing
- 2: Shipped
- 3: Delivered
- 4: Cancelled

### 10.2 Admin Order Views

- [GuhaStore.Web/Views/Admin/Order/Index.cshtml](GuhaStore.Web/Views/Admin/Order/Index.cshtml) - Order list with filters
- [GuhaStore.Web/Views/Admin/Order/Details.cshtml](GuhaStore.Web/Views/Admin/Order/Details.cshtml) - Order details
- Order status update interface

---

## Phase 11: Admin Dashboard & Analytics (Week 11)

### 11.1 Dashboard Controller

- [GuhaStore.Web/Controllers/Admin/DashboardController.cs](GuhaStore.Web/Controllers/Admin/DashboardController.cs)
  - `Index(DateTime? startDate, DateTime? endDate)` - Dashboard with statistics
  - Time-based filtering (7 days, 28 days, 90 days, 365 days)

### 11.2 Analytics Service

- [GuhaStore.Application/Services/AnalyticsService.cs](GuhaStore.Application/Services/AnalyticsService.cs)
  - `GetSalesMetricsAsync(DateTime startDate, DateTime endDate)` - Revenue, order count
  - `GetOrderStatisticsAsync(DateTime startDate, DateTime endDate)` - Order statistics
  - `GetTopProductsAsync(int count)` - Top selling products
  - Export functionality to Excel

### 11.3 Dashboard View

- [GuhaStore.Web/Views/Admin/Dashboard/Index.cshtml](GuhaStore.Web/Views/Admin/Dashboard/Index.cshtml)
  - Charts using Chart.js
  - Revenue statistics
  - Order counts
  - Top products list
  - Recent orders

---

## Phase 12: Additional Admin Modules (Week 12)

### 12.1 Category Management

- [GuhaStore.Web/Controllers/Admin/CategoryController.cs](GuhaStore.Web/Controllers/Admin/CategoryController.cs)
  - CRUD operations (Create, Read, Update, Delete)
  - Category listing with pagination

### 12.2 Brand Management

- [GuhaStore.Web/Controllers/Admin/BrandController.cs](GuhaStore.Web/Controllers/Admin/BrandController.cs)
  - CRUD operations
  - Brand listing

### 12.3 Collection Management

- [GuhaStore.Web/Controllers/Admin/CollectionController.cs](GuhaStore.Web/Controllers/Admin/CollectionController.cs)
  - CRUD operations

### 12.4 Customer Management

- [GuhaStore.Web/Controllers/Admin/CustomerController.cs](GuhaStore.Web/Controllers/Admin/CustomerController.cs)
  - Customer listing
  - Customer details view

### 12.5 Inventory Management

- [GuhaStore.Web/Controllers/Admin/InventoryController.cs](GuhaStore.Web/Controllers/Admin/InventoryController.cs)
  - Inventory tracking
  - Inventory details view
  - Stock management

### 12.6 Blog/Article Management

- [GuhaStore.Web/Controllers/Admin/ArticleController.cs](GuhaStore.Web/Controllers/Admin/ArticleController.cs)
  - Article CRUD operations
  - Article categories management
  - Comments management

### 12.7 Account Management (Admin)

- [GuhaStore.Web/Controllers/Admin/AccountController.cs](GuhaStore.Web/Controllers/Admin/AccountController.cs)
  - Admin account listing
  - Account editing
  - Account type management

---

## Phase 13: Frontend UI & Styling (Week 13)

### 13.1 Layout & Navigation

- [GuhaStore.Web/Views/Shared/_Layout.cshtml](GuhaStore.Web/Views/Shared/_Layout.cshtml) - Main layout
  - Header with navigation, search, cart icon
  - Footer with links and information
  - Responsive design implementation

### 13.2 Homepage

- [GuhaStore.Web/Views/Home/Index.cshtml](GuhaStore.Web/Views/Home/Index.cshtml)
  - Banner/carousel section
  - Featured products section
  - Product categories display
  - Blog/news section

### 13.3 Product Pages Styling

- [GuhaStore.Web/Views/Products/Index.cshtml](GuhaStore.Web/Views/Products/Index.cshtml) - Product listing with filters
- [GuhaStore.Web/Views/Products/Details.cshtml](GuhaStore.Web/Views/Products/Details.cshtml) - Product detail with image gallery
- Product card components
- Pagination styling

### 13.4 Cart & Checkout Styling

- [GuhaStore.Web/Views/Cart/Index.cshtml](GuhaStore.Web/Views/Cart/Index.cshtml) - Cart page layout
- [GuhaStore.Web/Views/Checkout/Index.cshtml](GuhaStore.Web/Views/Checkout/Index.cshtml) - Checkout form styling
- Order confirmation page styling

### 13.5 Admin Panel Styling

- Admin layout and sidebar
- Data tables styling
- Form styling
- Dashboard charts styling

### 13.6 JavaScript Functionality

- [GuhaStore.Web/wwwroot/js/site.js](GuhaStore.Web/wwwroot/js/site.js) - Custom JavaScript
- [GuhaStore.Web/wwwroot/js/cart.js](GuhaStore.Web/wwwroot/js/cart.js) - Cart AJAX operations
- Maintain jQuery for compatibility
- Form validation
- Toast notifications
- Image galleries
- Search functionality

---

## Phase 14: Additional Features (Week 14)

### 14.1 Blog/Article System (Frontend)

- [GuhaStore.Web/Controllers/BlogController.cs](GuhaStore.Web/Controllers/BlogController.cs) - Blog listing
- [GuhaStore.Web/Controllers/ArticleController.cs](GuhaStore.Web/Controllers/ArticleController.cs) - Article details
- Blog views and styling
- Comment system

### 14.2 Search Functionality

- Global search implementation
- Search results page
- Search suggestions (AJAX)

### 14.3 Product Filtering & Sorting

- Filter by category, brand, price range
- Sort by price, name, date
- Filter UI components

### 14.4 Email Notifications

- Order confirmation emails
- Order status update emails
- Password reset emails
- Email templates

### 14.5 File Upload Management

- Product image uploads
- Article image uploads
- Image validation and resizing
- File storage management

---

## Phase 15: Testing & Quality Assurance (Week 15)

### 15.1 Unit Testing

- Test service layer methods
- Test repository methods
- Test business logic

### 15.2 Integration Testing

- Test database operations
- Test API endpoints
- Test authentication flows

### 15.3 Manual Testing

- Test all user flows (browse, cart, checkout)
- Test admin panel functionality
- Test edge cases and error handling
- Cross-browser testing
- Responsive design testing

### 15.4 Bug Fixes

- Fix identified issues
- Performance optimization
- Security review
- Code refactoring

---

## Phase 16: Deployment & Documentation (Week 16)

### 16.1 Deployment Preparation

- Configure production settings in `appsettings.Production.json`
- Set up production MySQL database connection
- Configure email SMTP settings
- Set up file storage paths
- Environment configuration

### 16.2 Deployment

- Deploy to IIS or Azure
- Database connection to production MySQL
- SSL certificate setup
- Domain configuration

### 16.3 Documentation

- README.md with setup instructions
- Database schema documentation
- API documentation (if Web API)
- User manual for admin panel
- Deployment guide

---

## Technical Considerations

### Database Configuration

**MySql.EntityFrameworkCore Setup:**

- Use `UseMySQL()` extension method (not `UseMySql()`)
- Connection string format: `Server=localhost;Database=dbperfume;User=root;Password=;CharSet=utf8mb4;`
- Supports MySQL 8.0 and later
- Official Oracle support

**Entity Mapping:**

- Map MySQL snake_case column names to C# PascalCase properties
- Use `[Column("column_name")]` attribute or Fluent API `HasColumnName()`
- Example: `account_name` → `AccountName`

**Code-First with Existing Database:**

- Create entities matching existing schema
- Use Fluent API to map column names
- No migrations needed initially (using existing database)
- Use migrations only for schema changes

### Session Management

- Use ASP.NET Core Session middleware
- Store cart as JSON in session
- Configure session timeout (30 minutes)
- Session cookie settings (HttpOnly, IsEssential)

### Security Improvements

- Use BCrypt for password hashing (not MD5 from PHP)
- Implement prepared statements via EF Core (prevents SQL injection)
- Add CSRF protection
- Input validation and sanitization
- XSS protection
- Secure session management

### Performance Optimization

- Implement caching (MemoryCache for product catalog)
- Optimize database queries (use Include() for eager loading)
- Image optimization and CDN
- Lazy loading for images
- Database indexing (already exists in MySQL)

### Code Quality

- Follow SOLID principles
- Implement dependency injection
- Use async/await for I/O operations
- Error handling and logging
- Code documentation

---

## Implementation Order

The phases will be implemented in this order, with dependencies respected:

1. **Phase 1-2**: Core entities and interfaces (Foundation)
2. **Phase 3**: Infrastructure - Database access (Enables data operations)
3. **Phase 4**: Application services (Business logic)
4. **Phase 5**: DI Configuration (Wire everything together)
5. **Phase 6**: Authentication (Required for protected routes)
6. **Phase 7**: Orders (Core e-commerce functionality)
7. **Phase 8-12**: Customer and Admin features
8. **Phase 13-16**: Frontend, polish, testing, and deployment

---

## Estimated Timeline

- **Total Duration**: 16 weeks
- **Team Size**: 2-3 developers recommended
- **Phases can overlap** where dependencies allow

---

## Success Criteria

- All core functionality from PHP site working
- Admin panel fully functional
- COD payment processing working
- Responsive design implemented
- Performance meets or exceeds PHP version
- Security best practices implemented
- Code is maintainable and well-documented
- MySQL database connection working with MySql.EntityFrameworkCore
- All entities properly mapped to existing database schema