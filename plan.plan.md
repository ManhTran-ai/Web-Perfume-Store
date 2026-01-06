<!-- 4869a27c-daa8-4565-b46d-587650cc0e8b 25dcd3f0-826d-40c1-9ca5-9d54ac70882e -->
# PHP to .NET E-Commerce Migration Plan

## Project Overview

This plan outlines the complete migration of the PHP perfume e-commerce website to C# .NET. The migration will maintain all core functionality including product catalog, shopping cart, order management, admin panel, and customer accounts. Payment integration will be limited to Cash on Delivery (COD) only.

## Technology Stack

- **Backend**: ASP.NET Core (MVC or Web API + Razor Pages)
- **Database**: SQL Server (migrated from MySQL)
- **ORM**: Entity Framework Core
- **Frontend**: Razor Views + JavaScript/jQuery (maintaining similar UI)
- **Authentication**: ASP.NET Core Identity
- **Session Management**: ASP.NET Core Session
- **Email**: MailKit or System.Net.Mail
- **PDF Generation**: iTextSharp or QuestPDF
- **Excel Import/Export**: EPPlus or ClosedXML

---

## Phase 1: Project Setup & Infrastructure (Week 1-2)

### 1.1 Solution Structure Setup

- Create ASP.NET Core Web Application (MVC or Razor Pages)
- Set up solution structure:
  ```
  PerfumeECommerce/
  ├── PerfumeECommerce.Web/          # Main web application
  ├── PerfumeECommerce.Data/         # Data access layer (EF Core)
  ├── PerfumeECommerce.Models/       # Domain models
  ├── PerfumeECommerce.Services/     # Business logic services
  ├── PerfumeECommerce.Admin/        # Admin panel (separate area or project)
  └── PerfumeECommerce.Tests/        # Unit tests
  ```


### 1.2 Database Migration

- **Convert MySQL schema to SQL Server**
  - Export MySQL database structure
  - Create equivalent SQL Server database (`dbperfume`)
  - Map MySQL data types to SQL Server equivalents
  - Create Entity Framework Core DbContext
  - Define all entity models (Account, Product, Category, Brand, Order, OrderDetail, etc.)
  - Set up database migrations

### 1.3 Configuration Setup

- Configure `appsettings.json` for:
  - Database connection strings
  - Email SMTP settings
  - File upload paths
  - Session configuration
  - Application settings

### 1.4 Dependency Injection Setup

- Configure services in `Program.cs` or `Startup.cs`
- Set up repositories/services registration
- Configure Entity Framework Core DbContext
- Configure session middleware
- Configure authentication/authorization

**Key Files to Create:**

- `appsettings.json` - Configuration
- `DbContext.cs` - Database context
- `Program.cs` or `Startup.cs` - Service configuration

---

## Phase 2: Data Models & Database Layer (Week 2-3)

### 2.1 Entity Models Creation

Create Entity Framework Core models for all tables:

- `Account` (User accounts)
- `Product` (Products with variants)
- `Category` (Product categories)
- `Brand` (Product brands)
- `Collection` (Product collections)
- `Capacity` (Product capacity variants)
- `Order` (Orders)
- `OrderDetail` (Order line items)
- `Customer` (Customer information)
- `Delivery` (Delivery information)
- `Article` (Blog articles)
- `Comment` (Product comments)
- `Evaluate` (Product ratings)
- `Inventory` (Inventory records)
- `InventoryDetail` (Inventory details)

### 2.2 Repository Pattern Implementation

- Create generic repository interface and implementation
- Create specific repositories:
  - `IProductRepository` / `ProductRepository`
  - `IOrderRepository` / `OrderRepository`
  - `ICategoryRepository` / `CategoryRepository`
  - `IAccountRepository` / `AccountRepository`
  - etc.

### 2.3 Database Seeding

- Create seed data for initial setup
- Migrate existing data from MySQL (if needed)
- Set up database initializer

**Key Files:**

- `Models/Account.cs`, `Models/Product.cs`, etc.
- `Data/ApplicationDbContext.cs`
- `Repositories/IProductRepository.cs`, `Repositories/ProductRepository.cs`

---

## Phase 3: Authentication & Authorization (Week 3-4)

### 3.1 ASP.NET Core Identity Setup

- Configure Identity with custom `ApplicationUser`
- Set up password hashing (BCrypt/Argon2, not MD5)
- Create login/register functionality
- Implement forgot password feature
- Email confirmation (if needed)

### 3.2 Role-Based Authorization

- Create roles: Admin, Customer
- Set up role-based access control
- Protect admin routes with `[Authorize(Roles = "Admin")]`
- Create admin login page

### 3.3 Session Management

- Configure session middleware
- Create cart service using session storage
- Implement session-based shopping cart
- Handle cart persistence across requests

**Key Files:**

- `Models/ApplicationUser.cs`
- `Services/CartService.cs`
- `Controllers/AccountController.cs`
- `Areas/Admin/Controllers/AdminAccountController.cs`

---

## Phase 4: Core Product Management (Week 4-5)

### 4.1 Product Service Layer

- Create `IProductService` and `ProductService`
- Implement product CRUD operations
- Product search functionality
- Product filtering by category, brand, collection
- Product variant management (capacity/size)

### 4.2 Product Controllers & Views

- `ProductController` with actions:
  - `Index()` - Product listing
  - `Details(int id)` - Product detail page
  - `Search(string query)` - Search functionality
  - `Filter()` - Filter products
- Create Razor views:
  - `Products/Index.cshtml` - Product listing
  - `Products/Details.cshtml` - Product detail
  - `Products/Search.cshtml` - Search results

### 4.3 Product Display Features

- Product image gallery
- Product variants selection (capacity)
- Product recommendations
- Related products display
- Product ratings and reviews display

**Key Files:**

- `Services/IProductService.cs`, `Services/ProductService.cs`
- `Controllers/ProductController.cs`
- `Views/Products/Index.cshtml`, `Views/Products/Details.cshtml`

---

## Phase 5: Shopping Cart Implementation (Week 5-6)

### 5.1 Cart Service Implementation

- Create `ICartService` and `CartService`
- Methods:
  - `AddToCart(int productId, int quantity, int? capacityId)`
  - `UpdateCartItem(string variantKey, int quantity)`
  - `RemoveFromCart(string variantKey)`
  - `GetCartItems()` - Retrieve cart from session
  - `ClearCart()` - Clear cart after order
  - `GetCartTotal()` - Calculate total amount

### 5.2 Cart Controller & Views

- `CartController` with actions:
  - `Index()` - Display cart
  - `AddToCart()` - Add product to cart
  - `UpdateQuantity()` - Update item quantity
  - `RemoveItem()` - Remove item from cart
- Create `Views/Cart/Index.cshtml` - Cart display page
- Implement AJAX calls for cart operations

### 5.3 Cart Features

- Session-based cart storage
- Variant support (product + capacity)
- Quantity validation against inventory
- Real-time cart total calculation
- Cart item count in header

**Key Files:**

- `Services/ICartService.cs`, `Services/CartService.cs`
- `Controllers/CartController.cs`
- `Views/Cart/Index.cshtml`
- `wwwroot/js/cart.js` - Cart JavaScript

---

## Phase 6: Order Management - COD Only (Week 6-7)

### 6.1 Order Service Layer

- Create `IOrderService` and `OrderService`
- Methods:
  - `CreateOrder(OrderViewModel model)` - Create new order
  - `GetOrderById(int orderId)` - Get order details
  - `GetUserOrders(int userId)` - Get user's orders
  - `UpdateOrderStatus(int orderId, OrderStatus status)`
  - `ValidateCartInventory()` - Check stock availability

### 6.2 Checkout Process

- `CheckoutController` with actions:
  - `Index()` - Display checkout form
  - `ProcessOrder()` - Process COD order
  - `OrderConfirmation(int orderId)` - Order confirmation page
- Create checkout view with:
  - Delivery information form
  - Order summary
  - COD payment option only
- Order validation (inventory check, cart validation)

### 6.3 Order Processing

- Generate unique order code
- Create order record in database
- Create order detail records
- Update inventory quantities
- Clear shopping cart
- Send order confirmation email
- Redirect to thank you page

**Key Files:**

- `Services/IOrderService.cs`, `Services/OrderService.cs`
- `Controllers/CheckoutController.cs`
- `Views/Checkout/Index.cshtml`
- `Views/Checkout/OrderConfirmation.cshtml`
- `Services/EmailService.cs` - Email notifications

---

## Phase 7: Customer Account Management (Week 7-8)

### 7.1 Account Controller

- `AccountController` with actions:
  - `MyAccount()` - Account dashboard
  - `OrderHistory()` - Display user orders
  - `OrderDetail(int orderId)` - Order details
  - `AccountInfo()` - Account information
  - `UpdateAccountInfo()` - Update account
  - `ChangePassword()` - Change password

### 7.2 Account Views

- `Views/Account/MyAccount.cshtml` - Account dashboard
- `Views/Account/OrderHistory.cshtml` - Order list
- `Views/Account/OrderDetail.cshtml` - Order details
- `Views/Account/AccountInfo.cshtml` - Account info form
- `Views/Account/ChangePassword.cshtml` - Password change form

**Key Files:**

- `Controllers/AccountController.cs`
- `Views/Account/*.cshtml`

---

## Phase 8: Admin Panel - Product Management (Week 8-9)

### 8.1 Admin Area Setup

- Create `Areas/Admin` structure
- Set up admin layout and navigation
- Configure admin routing
- Protect admin area with authorization

### 8.2 Product Management (Admin)

- `Admin/ProductController` with actions:
  - `Index()` - Product list with pagination
  - `Create()` - Add new product
  - `Edit(int id)` - Edit product
  - `Delete(int id)` - Delete product
  - `Search()` - Search products
  - `Import()` - Import from Excel
  - `Export()` - Export to Excel
- Create admin product views
- Implement file upload for product images
- Product variant management (capacity)

**Key Files:**

- `Areas/Admin/Controllers/ProductController.cs`
- `Areas/Admin/Views/Product/*.cshtml`
- `Services/IProductService.cs` (extend for admin operations)

---

## Phase 9: Admin Panel - Order Management (Week 9-10)

### 9.1 Order Management (Admin)

- `Admin/OrderController` with actions:
  - `Index()` - Order list
  - `Details(int id)` - Order details
  - `UpdateStatus()` - Update order status
  - `Search()` - Search orders
  - `Create()` - Create direct order
- Order status management (Pending, Processing, Shipped, Delivered, Cancelled)
- Order filtering and sorting

### 9.2 Order Views

- `Views/Admin/Order/Index.cshtml` - Order list
- `Views/Admin/Order/Details.cshtml` - Order details
- Order status update interface

**Key Files:**

- `Areas/Admin/Controllers/OrderController.cs`
- `Areas/Admin/Views/Order/*.cshtml`

---

## Phase 10: Admin Panel - Dashboard & Analytics (Week 10-11)

### 10.1 Dashboard Implementation

- `Admin/DashboardController` with `Index()` action
- Create dashboard view with:
  - Order statistics (revenue, order count, quantities)
  - Time-based filtering (7 days, 28 days, 90 days, 365 days)
  - Chart visualizations (using Chart.js or similar)
  - Recent orders list
  - Top products

### 10.2 Analytics Service

- Create `IAnalyticsService` and `AnalyticsService`
- Methods:
  - `GetSalesMetrics(DateTime startDate, DateTime endDate)`
  - `GetOrderStatistics(DateTime startDate, DateTime endDate)`
  - `GetTopProducts(int count)`
- Export functionality to Excel

**Key Files:**

- `Areas/Admin/Controllers/DashboardController.cs`
- `Areas/Admin/Views/Dashboard/Index.cshtml`
- `Services/IAnalyticsService.cs`, `Services/AnalyticsService.cs`

---

## Phase 11: Admin Panel - Additional Modules (Week 11-12)

### 11.1 Category Management

- `Admin/CategoryController` - CRUD operations
- Category listing, add, edit, delete

### 11.2 Brand Management

- `Admin/BrandController` - CRUD operations
- Brand listing, add, edit, delete

### 11.3 Collection Management

- `Admin/CollectionController` - CRUD operations
- Collection listing, add, edit, delete

### 11.4 Customer Management

- `Admin/CustomerController` - Customer listing
- Customer details view

### 11.5 Inventory Management

- `Admin/InventoryController` - Inventory tracking
- Inventory details and stock management

### 11.6 Blog/Article Management

- `Admin/ArticleController` - Article CRUD
- Article categories and comments management

### 11.7 Account Management (Admin)

- `Admin/AccountController` - Admin account management
- Account listing and editing

**Key Files:**

- `Areas/Admin/Controllers/*Controller.cs` for each module
- `Areas/Admin/Views/*/*.cshtml` for each module

---

## Phase 12: Frontend UI & Styling (Week 12-13)

### 12.1 Layout & Navigation

- Create main layout (`_Layout.cshtml`)
- Header with navigation, search, cart icon
- Footer with links and information
- Responsive design implementation

### 12.2 Homepage

- Banner/carousel section
- Featured products section
- Product categories display
- Blog/news section

### 12.3 Product Pages Styling

- Product listing page with filters
- Product detail page with image gallery
- Product card components
- Pagination styling

### 12.4 Cart & Checkout Styling

- Cart page layout
- Checkout form styling
- Order confirmation page

### 12.5 Admin Panel Styling

- Admin layout and sidebar
- Data tables styling
- Form styling
- Dashboard charts styling

### 12.6 JavaScript Functionality

- Maintain jQuery for compatibility
- Cart AJAX operations
- Form validation
- Toast notifications
- Image galleries
- Search functionality

**Key Files:**

- `Views/Shared/_Layout.cshtml`
- `Views/Home/Index.cshtml`
- `wwwroot/css/site.css` - Custom styles
- `wwwroot/js/site.js` - Custom JavaScript

---

## Phase 13: Additional Features (Week 13-14)

### 13.1 Blog/Article System (Frontend)

- `BlogController` - Blog listing
- `ArticleController` - Article details
- Blog views and styling

### 13.2 Search Functionality

- Global search implementation
- Search results page
- Search suggestions

### 13.3 Product Filtering & Sorting

- Filter by category, brand, price range
- Sort by price, name, date
- Filter UI components

### 13.4 Email Notifications

- Order confirmation emails
- Order status update emails
- Password reset emails
- Email templates

### 13.5 File Upload Management

- Product image uploads
- Image validation and resizing
- File storage management

**Key Files:**

- `Controllers/BlogController.cs`, `Controllers/ArticleController.cs`
- `Services/IEmailService.cs`, `Services/EmailService.cs`
- `Services/IFileService.cs`, `Services/FileService.cs`

---

## Phase 14: Testing & Quality Assurance (Week 14-15)

### 14.1 Unit Testing

- Test service layer methods
- Test repository methods
- Test business logic

### 14.2 Integration Testing

- Test database operations
- Test API endpoints
- Test authentication flows

### 14.3 Manual Testing

- Test all user flows (browse, cart, checkout)
- Test admin panel functionality
- Test edge cases and error handling
- Cross-browser testing
- Responsive design testing

### 14.4 Bug Fixes

- Fix identified issues
- Performance optimization
- Security review

---

## Phase 15: Deployment & Documentation (Week 15-16)

### 15.1 Deployment Preparation

- Configure production settings
- Set up production database
- Configure email settings
- Set up file storage
- Environment configuration

### 15.2 Deployment

- Deploy to IIS or Azure
- Database migration to production
- SSL certificate setup
- Domain configuration

### 15.3 Documentation

- API documentation (if Web API)
- User manual for admin panel
- Deployment guide
- Database schema documentation

---

## Technical Considerations

### Database Migration Strategy

1. Export MySQL data to CSV/JSON
2. Create SQL Server schema using EF Core migrations
3. Import data using data migration scripts
4. Verify data integrity

### Session Management

- Use ASP.NET Core Session middleware
- Store cart as JSON in session
- Configure session timeout
- Consider Redis for distributed sessions (if needed)

### Security Improvements

- Use BCrypt/Argon2 for password hashing (not MD5)
- Implement prepared statements via EF Core (prevents SQL injection)
- Add CSRF protection
- Input validation and sanitization
- XSS protection

### Performance Optimization

- Implement caching (MemoryCache, Redis)
- Optimize database queries
- Image optimization and CDN
- Lazy loading for images
- Database indexing

### Code Quality

- Follow SOLID principles
- Implement dependency injection
- Use async/await for I/O operations
- Error handling and logging
- Code documentation

---

## Estimated Timeline

- **Total Duration**: 15-16 weeks
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

### To-dos

- [ ] Phase 1: Set up .NET solution structure, configure database migration from MySQL to SQL Server, and configure dependency injection
- [ ] Phase 2: Create all Entity Framework Core models and implement repository pattern for data access
- [ ] Phase 3: Implement ASP.NET Core Identity for authentication, role-based authorization, and session management
- [ ] Phase 4: Build product service layer, controllers, and views for product catalog functionality
- [ ] Phase 5: Implement shopping cart service with session storage, cart controller, and cart views
- [ ] Phase 6: Build order service and checkout process with COD payment only
- [ ] Phase 7: Create customer account management (order history, account info, password change)
- [ ] Phase 8: Build admin panel product management module with CRUD, import/export
- [ ] Phase 9: Implement admin order management with status updates and order details
- [ ] Phase 10: Create admin dashboard with analytics, charts, and statistics
- [ ] Phase 11: Build remaining admin modules (category, brand, collection, customer, inventory, blog, accounts)
- [ ] Phase 12: Implement frontend UI, styling, layouts, and JavaScript functionality
- [ ] Phase 13: Add blog system, search, filtering, email notifications, and file uploads
- [ ] Phase 14: Conduct unit testing, integration testing, manual testing, and bug fixes
- [ ] Phase 15: Prepare for deployment, deploy to production, and create documentation