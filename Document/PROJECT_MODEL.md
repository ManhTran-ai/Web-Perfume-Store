# Perfume E-Commerce Website - Project Model

## Project Overview

This is an ASP.NET Core MVC-based e-commerce website for selling perfumes (nước hoa). The project follows Clean Architecture pattern with separate layers for Core, Infrastructure, Application, and Web. The system supports product management, order processing, payment integration (MoMo, VNPay), inventory management, and customer account management.

**Project Name:** GuhaStore - Web Bán Nước Hoa (Perfume Sales Website)  
**Technology Stack:** C# .NET 8, ASP.NET Core MVC, Entity Framework Core, MySQL  
**Database:** MySQL (dbperfume)  
**Architecture:** Clean Architecture (Domain-Driven Design)

---

## Project Structure

```
GuhaStore/
├── GuhaStore.sln                    # Solution file
├── GuhaStore.Core/                  # Domain Layer - Entities & Interfaces
│   ├── Entities/                    # Domain entities
│   │   ├── Account.cs
│   │   ├── Article.cs
│   │   ├── Brand.cs
│   │   ├── Capacity.cs
│   │   ├── Category.cs
│   │   ├── Collection.cs
│   │   ├── Comment.cs
│   │   ├── Customer.cs
│   │   ├── Delivery.cs
│   │   ├── Evaluate.cs
│   │   ├── Inventory.cs
│   │   ├── InventoryDetail.cs
│   │   ├── Metric.cs
│   │   ├── Order.cs
│   │   ├── OrderDetail.cs
│   │   ├── Product.cs
│   │   └── ProductVariant.cs
│   └── Interfaces/                  # Service & Repository interfaces
│       ├── IAnalyticsService.cs
│       ├── IArticleService.cs
│       ├── ICartService.cs
│       ├── IEmailService.cs
│       ├── IFileUploadService.cs
│       ├── IInventoryService.cs
│       ├── IOrderService.cs
│       ├── IProductService.cs
│       ├── IRepository.cs
│       └── IUnitOfWork.cs
├── GuhaStore.Infrastructure/        # Infrastructure Layer - Data Access
│   ├── Data/
│   │   └── ApplicationDbContext.cs  # EF Core DbContext
│   └── Repositories/
│       ├── Repository.cs            # Generic Repository
│       └── UnitOfWork.cs            # Unit of Work pattern
├── GuhaStore.Application/           # Application Layer - Business Logic
│   └── Services/
│       ├── AnalyticsService.cs
│       ├── ArticleService.cs
│       ├── CartService.cs
│       ├── EmailService.cs
│       ├── FileUploadService.cs
│       ├── InventoryService.cs
│       ├── OrderService.cs
│       └── ProductService.cs
├── GuhaStore.Web/                   # Presentation Layer - MVC Web App
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── BlogController.cs
│   │   ├── CartController.cs
│   │   ├── CheckoutController.cs
│   │   ├── ContactController.cs
│   │   ├── HomeController.cs
│   │   ├── ProductsController.cs
│   │   └── Admin/                   # Admin controllers
│   │       ├── ArticleController.cs
│   │       └── InventoryController.cs
│   ├── Views/                       # Razor Views
│   │   ├── Account/
│   │   ├── Article/
│   │   ├── Blog/
│   │   ├── Cart/
│   │   ├── Checkout/
│   │   ├── Contact/
│   │   ├── Home/
│   │   ├── Inventory/
│   │   ├── Products/
│   │   ├── Shared/                  # Layout, partials
│   │   ├── _ViewImports.cshtml
│   │   └── _ViewStart.cshtml
│   ├── Models/                      # ViewModels
│   ├── Filters/                     # Action Filters
│   ├── Middleware/                  # Custom Middleware
│   ├── wwwroot/                     # Static files (CSS, JS, Images)
│   ├── Program.cs                   # Application entry point
│   ├── appsettings.json            # Configuration
│   └── appsettings.Development.json
├── fonts/                           # Custom fonts
├── dbperfume.sql                   # Database schema & seed data
└── create_product_variants_table.sql
```

---

## Key Features

### Frontend Features

1. **Product Catalog**
   - Product listing with filtering and sorting
   - Product detail pages with images
   - Product search functionality
   - Category and brand browsing
   - Product recommendations

2. **Shopping Cart**
   - Add to cart functionality
   - Cart management (update quantities, remove items)
   - Cart persistence using Sessions

3. **Checkout & Payment**
   - Checkout process
   - Multiple payment methods:
      - Cash on Delivery (COD)
   - Order confirmation

4. **User Account Management**
   - User registration and login
   - Account information management
   - Order history
   - Password change functionality
   - Forgot password feature

5. **Content Pages**
   - Blog/Article system
   - About page
   - Contact page
   - Product filtering and sorting

6. **Additional Features**
   - Toast notifications
   - Responsive design
   - Product ratings and reviews

### Admin Panel Features

1. **Dashboard**
   - Order statistics and analytics
   - Sales metrics (revenue, orders, quantities)
   - Time-based filtering
   - Chart visualizations

2. **Product Management**
   - Add, edit, delete products
   - Product listing with pagination
   - Product search
   - Inventory management
   - Product variants (capacity, etc.)

3. **Order Management**
   - Order listing
   - Order details view
   - Order status management
   - Direct order creation
   - Order search

4. **Category & Brand Management**
   - Category CRUD operations
   - Brand CRUD operations
   - Collection management

5. **Content Management**
   - Blog/Article management
   - Article categories
   - Comments management

6. **Customer Management**
   - Customer listing
   - Customer information management

7. **Inventory Management**
   - Inventory tracking
   - Inventory details
   - Stock management

8. **Account Management**
   - Admin account management
   - Password change

---

## Database Schema

The database `dbperfume` contains the following main tables:

### Core Tables

1. **account** - User accounts (customers and admins)
   - `account_id`, `account_name`, `account_password`, `account_email`, `account_phone`, `account_type`, `account_status`

2. **product** - Product information
   - `product_id`, `product_name`, `product_category`, `product_brand`, `capacity_id`, `product_quantity`, `quantity_sales`, `product_price_import`, `product_price`, `product_sale`, `product_description`, `product_image`, `product_status`

3. **product_variants** - Product size/capacity variants
   - Variant pricing and stock per capacity

4. **category** - Product categories
   - `category_id`, `category_name`, `category_description`, `category_image`

5. **brand** - Product brands
   - Brand information

6. **collection** - Product collections
   - Collection grouping

7. **orders** - Order information
   - Order details, customer info, totals, status

8. **order_detail** - Order line items
   - Products in each order, quantities, prices

9. **customer** - Customer information
   - Customer details, addresses, contact info

10. **inventory** - Inventory records
    - Stock tracking

11. **inventory_detail** - Inventory line items
    - Detailed inventory information

12. **article** - Blog articles
    - Article content, images, dates, status

13. **comment** - Product comments/reviews
    - Customer feedback

14. **evaluate** - Product evaluations/ratings
    - Rating system

15. **delivery** - Delivery information
    - Shipping details

16. **capacity** - Product capacity variants
    - Different sizes/volumes for products

17. **metrics** - Analytics metrics
    - Statistical data

---

## Technology Stack & Libraries

### Backend
- **ASP.NET Core 8** - Web framework
- **Entity Framework Core** - ORM (Object-Relational Mapping)
- **MySQL** - Database (via MySql.EntityFrameworkCore)
- **Dependency Injection** - Built-in DI container

### Architecture Patterns
- **Clean Architecture** - Separation of concerns
- **Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management
- **Service Layer** - Business logic encapsulation

### Frontend
- **Razor Views** - Server-side rendering
- **Bootstrap** - CSS framework
- **jQuery** - JavaScript library
- **Custom CSS/JS** - Custom styling and functionality

### Key NuGet Packages
- `Microsoft.EntityFrameworkCore`
- `MySql.EntityFrameworkCore`
- `Microsoft.AspNetCore.Session`
- `Microsoft.AspNetCore.Mvc`

---

## Configuration

### Database Configuration
Located in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=dbperfume;User=root;Password=;CharSet=utf8mb4;"
  }
}
```

### Service Registration
Located in `Program.cs`:
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

// Register UnitOfWork and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Application Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
```

---

## Routing System

### MVC Routing
The application uses ASP.NET Core MVC routing:

**Default Route Pattern:**
```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### Frontend Routes
| URL Pattern | Controller | Action | Description |
|-------------|------------|--------|-------------|
| `/` | HomeController | Index | Homepage |
| `/Products` | ProductsController | Index | Product listing |
| `/Products/Detail/{id}` | ProductsController | Detail | Product detail |
| `/Cart` | CartController | Index | Shopping cart |
| `/Checkout` | CheckoutController | Index | Checkout page |
| `/Account/Login` | AccountController | Login | User login |
| `/Account/Register` | AccountController | Register | User registration |
| `/Blog` | BlogController | Index | Blog listing |
| `/Contact` | ContactController | Index | Contact page |

### Admin Routes
| URL Pattern | Controller | Action | Description |
|-------------|------------|--------|-------------|
| `/Admin/Article` | Admin/ArticleController | Index | Article management |
| `/Admin/Inventory` | Admin/InventoryController | Index | Inventory management |

---

## Key Components Description

### Core Layer (GuhaStore.Core)

**Entities** - Domain models representing database tables:
- `Product` - Product information with navigation properties
- `Category`, `Brand`, `Capacity` - Product categorization
- `Order`, `OrderDetail` - Order management
- `Account`, `Customer` - User management
- `Article`, `Comment`, `Evaluate` - Content & feedback

**Interfaces** - Contracts for services and repositories:
- `IRepository<T>` - Generic repository interface
- `IUnitOfWork` - Unit of work interface
- `IProductService`, `IOrderService`, etc. - Service interfaces

### Infrastructure Layer (GuhaStore.Infrastructure)

**ApplicationDbContext** - Entity Framework Core DbContext:
- Configures entity mappings
- Manages database connections
- Handles migrations

**Repository & UnitOfWork** - Data access implementation:
- Generic repository for CRUD operations
- Unit of Work for transaction management

### Application Layer (GuhaStore.Application)

**Services** - Business logic implementation:
- `ProductService` - Product operations
- `CartService` - Shopping cart management (Session-based)
- `OrderService` - Order processing
- `InventoryService` - Stock management
- `EmailService` - Email notifications
- `AnalyticsService` - Dashboard analytics
- `FileUploadService` - Image upload handling

### Web Layer (GuhaStore.Web)

**Controllers** - Handle HTTP requests:
- Process user input
- Call application services
- Return appropriate views

**Views** - Razor templates:
- Display data to users
- Forms for user input

**Middleware** - Request pipeline customization:
- Error handling
- Custom processing

---

## Payment Integration


### Cash on Delivery
- COD option for customers
- Order processing without online payment

---

## Security Considerations

1. **Authentication**: Session-based authentication
2. **Password Hashing**: Should use ASP.NET Core Identity or BCrypt
3. **Input Validation**: Model validation with Data Annotations
4. **SQL Injection Prevention**: Entity Framework parameterized queries
5. **XSS Prevention**: Razor automatic HTML encoding
6. **CSRF Protection**: Anti-forgery tokens

---

## Development Setup

### Prerequisites
- .NET 8 SDK
- MySQL 8.0+
- Visual Studio 2022 / JetBrains Rider / VS Code

### Setup Steps
1. Clone the repository
2. Import `dbperfume.sql` into MySQL
3. Update connection string in `appsettings.json`
4. Run `dotnet restore` to restore packages
5. Run `dotnet run --project GuhaStore.Web` to start the application

### Running with Scripts
```powershell
# Start application
.\run-app.ps1

# Stop application
.\stop-app.ps1
```

---

## Deployment Considerations

1. **Server Requirements**
   - .NET 8 Runtime
   - MySQL 8.0+
   - IIS / Nginx / Kestrel

2. **Configuration Needed**
   - Update database connection string
   - Configure payment gateway credentials
   - Set up email SMTP settings
   - Configure HTTPS certificates

3. **Environment Variables**
   - Use User Secrets for development
   - Use Environment Variables for production

---

## Future Enhancement Opportunities

1. **Security Improvements**
   - Implement ASP.NET Core Identity
   - Add JWT authentication for API
   - Implement role-based authorization
   - Add two-factor authentication

2. **Feature Additions**
   - Wishlist functionality
   - Product comparison
   - Advanced search with Elasticsearch
   - Multi-language support (localization)
   - Mobile API (REST/GraphQL)
   - Real-time notifications (SignalR)

3. **Performance Optimization**
   - Implement caching (Redis/Memory Cache)
   - Image optimization & CDN
   - Database query optimization
   - Response compression

4. **Code Quality**
   - Add unit tests (xUnit/NUnit)
   - Integration tests
   - API documentation (Swagger)
   - Logging (Serilog)
   - Health checks

5. **DevOps**
   - Docker containerization
   - CI/CD pipeline
   - Azure/AWS deployment

---

## Summary

GuhaStore is a comprehensive e-commerce platform for perfume sales built with:
- **ASP.NET Core 8 MVC** - Modern web framework
- **Clean Architecture** - Maintainable code structure
- **Entity Framework Core** - Robust ORM
- **MySQL Database** - Reliable data storage
- **Repository & Unit of Work Patterns** - Clean data access
- **Dependency Injection** - Loosely coupled components

Features include:
- Full-featured customer frontend
- Admin panel for management
- Multiple payment gateway integrations
- Inventory and order management
- Content management system (blog)
- Analytics and reporting
- Email notifications

The project follows best practices for ASP.NET Core development with clear separation of concerns and maintainable architecture.
