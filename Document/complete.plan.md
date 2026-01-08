<!-- fb773890-be14-470c-9a0f-7a3d242461ac 8e55503d-72ce-4c1d-8c4b-d1036bcb7912 -->
# Implementation Plan: Complete Missing Features for GuhaStore

This plan breaks the remaining work into 6 phases (discoverable, incremental) with backend logic and frontend UI/UX tasks per feature. I list concrete files to modify and high-level implementation notes so you can accept and begin implementation.

---

## Phase 0 — Preparation & Infra (1-2 days)

- Goal: Ensure development environment, coding standards, and shared services are ready.
- Backend
- Add missing `DashboardController` skeleton and admin area routing. Edit `GuhaStore.Web/Program.cs` to ensure Admin area routing and policies are configured.
- Add Serilog logging dependency and initialize in `Program.cs`.
- Add `appsettings.Development.json` placeholders for payment credentials and SMTP settings.
- Frontend
- Create base admin layout `GuhaStore.Web/Views/Shared/_LayoutAdmin.cshtml` and SCSS/CSS file.
- Sketch a simple admin sidebar and responsive container.
- Files to touch (examples)
- `GuhaStore.Web/Program.cs`
- `GuhaStore.Web/Areas/Admin/Controllers/DashboardController.cs` (new)
- `GuhaStore.Web/Views/Shared/_LayoutAdmin.cshtml` (new)

## Phase 1 — Admin Panel Core (2 weeks)

- Goal: Implement full admin CRUD for Products, Orders, Categories, Brands, Customers and Dashboard analytics.
- Backend
- Add Admin controllers and views:
  - `GuhaStore.Web/Controllers/Admin/ProductAdminController.cs` (CRUD)
  - `GuhaStore.Web/Controllers/Admin/OrderAdminController.cs` (order list, detail, status updates)
  - `GuhaStore.Web/Controllers/Admin/CategoryAdminController.cs`
  - `GuhaStore.Web/Controllers/Admin/BrandAdminController.cs`
  - `GuhaStore.Web/Controllers/Admin/CustomerAdminController.cs`
- Extend `IUnitOfWork` and repositories (if missing) to support paged queries, filters and status updates (`GetPagedAsync`, `UpdateStatusAsync`).
- Add `Admin` area routes and view models (`GuhaStore.Web/Models/Admin/*ViewModel.cs`).
- Dashboard: use `AnalyticsService` methods to implement `DashboardController.Index` that returns sales, top products, pending orders.
- Ensure transactional updates when changing order status and inventory via `OrderService` and `UnitOfWork`.
- Frontend (UI/UX)
- Build admin list views with server-side pagination, search, filters, and bulk actions.
- Product edit view: support product images upload, product variants management (capacity + price + stock). Use partials: `Views/Admin/Products/_VariantRow.cshtml`.
- Order detail view: show order timeline, status change dropdown, and logs.
- Files to touch
- `GuhaStore.Application/Services/ProductService.cs` (extend for admin operations)
- `GuhaStore.Web/Controllers/Admin/ProductAdminController.cs` (new)
- `GuhaStore.Web/Views/Admin/ProductAdmin/*` (new views)

## Phase 2 — Checkout & COD Payment (1 week)

- Goal: Complete the checkout flow with Cash on Delivery (COD) payment method.
- Backend
- Ensure `OrderService` handles COD orders correctly: validate cart, create order with `PaymentMethod = "COD"`, update inventory, clear cart.
- Add order status workflow: Pending → Confirmed → Shipping → Delivered → Completed (or Cancelled).
- Add email notification service to send order confirmation email to customer after checkout.
- Implement order validation: check product availability, validate customer information, calculate shipping fee.
- Add `CheckoutController` actions: `Index` (checkout form), `PlaceOrder` (process order), `Confirmation` (success page).
- Frontend (UI/UX)
- Checkout page: customer information form (name, phone, address, email, notes).
- Display cart summary with product details, quantities, prices, and total.
- Show COD payment method as default (with icon and description).
- Order confirmation page: show order code, order details, estimated delivery time, and thank you message.
- Add form validation (client-side and server-side) for required fields.
- Files to touch
- `GuhaStore.Application/Services/OrderService.cs` (extend checkout logic)
- `GuhaStore.Application/Services/EmailService.cs` (order confirmation email)
- `GuhaStore.Web/Controllers/CheckoutController.cs` (checkout flow)
- `GuhaStore.Web/Views/Checkout/Index.cshtml` (checkout form)
- `GuhaStore.Web/Views/Checkout/Confirmation.cshtml` (success page)
- `GuhaStore.Web/Models/CheckoutViewModel.cs` (view model for checkout)

## Phase 3 — Reviews, Wishlist, Comments (1 week)

- Goal: Surface product evaluations and allow wishlist and comments on articles/products.
- Backend
- Ensure `Evaluate` and `Comment` entities are fully used. Add endpoints/services methods in `IProductService`/`IArticleService`: `AddReviewAsync`, `GetReviewsForProductAsync`, `AddCommentAsync`.
- Add wishlist persistence table `wishlist` and `WishlistService` (or store in session + option to persist for logged users).
- Frontend (UI/UX)
- Product detail: add review form (rating stars, text), show paged reviews, aggregate average rating, microdata for SEO.
- Add wishlist heart icon on product cards (`Views/Products/_ProductCard.cshtml`) and a `Wishlist` page.
- Article pages: comment form below article and moderation queue in Admin.
- Files to touch
- `GuhaStore.Application/Services/EvaluateService.cs` (new/extend)
- `GuhaStore.Web/Views/Products/Details.cshtml` (add review UI)
- `GuhaStore.Web/Views/Products/_ProductCard.cshtml` (wishlist button)

## Phase 4 — UX Enhancements & Frontend polish (1-2 weeks)

- Goal: Improve customer UX: responsive design, product images, variant selector, advanced filters, toast notifications.
- Frontend
- Implement responsive grid for product listing and product gallery with zoom/lightbox.
- Variant selector UI: when customer picks capacity, update price/stock via AJAX calling `IProductService.GetProductVariantAsync`.
- Advanced filters UI: price slider, on-sale toggle, brand/category checkboxes (use existing controller query params).
- Toast notifications: integrate a simple JS toast component and wire TempData messages to toasts.
- Improve header: cart count live update via AJAX to `CartController.GetCartCount`.
- Backend
- Add small endpoints returning JSON for variant lookup and wishlist toggle (e.g., `ProductsController/VariantInfo`, `WishlistController/Toggle`).
- Files to touch
- `wwwroot/js/site.js` (toast & UI behaviors)
- `GuhaStore.Web/Views/Shared/_Layout.cshtml` (header/cart updates)
- `GuhaStore.Web/Views/Products/Index.cshtml` (filters markup)

## Phase 5 — Security, Tests, and Performance (2 weeks)

- Goal: Harden auth, add tests, caching and observability.
- Backend
- Migrate auth to ASP.NET Core Identity or at least wrap account operations to use Identity-compatible password hashing and role management. Add role-based policies in `Program.cs` and secure admin area endpoints.
- Add unit tests (xUnit) for `ProductService`, `CartService`, `OrderService` and integration tests for checkout flow.
- Add caching for product lists (MemoryCache or Redis) in `ProductService.GetActiveProductsAsync`.
- Add Serilog logging and health checks (`/health` endpoint).
- Frontend
- Review CSRF tokens, input validation and encode user content (already using Razor). Add client-side validation using unobtrusive validation for forms.
- Files to touch
- `GuhaStore.Application.Tests/*` (new test project)
- `Program.cs` (identity & policies)
- `GuhaStore.Application/Services/ProductService.cs` (caching)

## Phase 6 — DevOps & Production Readiness (1 week)

- Goal: Dockerize, CI/CD, and prepare deployment checklist.
- Tasks
- Add `Dockerfile` for `GuhaStore.Web` and `docker-compose.yml` with MySQL. Add database initialization script (use `dbperfume.sql`).
- Add GitHub Actions pipeline: build, test, and produce Docker images.
- Prepare production `appsettings.Production.json` and secrets management guide.
- Add README updates and deployment docs.

---

## Acceptance criteria (per phase)

- Admin Panel Core: Admin can CRUD products, categories, brands, orders and see dashboard metrics. Tests for at least product admin APIs.
- Checkout & COD: Complete checkout flow works end-to-end with COD payment, order confirmation email sent, inventory updated correctly.
- Reviews/Wishlist: Users can add reviews and wishlist items and admin can moderate comments.
- UX: Responsive product pages, variant selector updates price dynamically, toasts, live cart count.
- Security & Tests: Role-based admin access, automated tests, caching, logging.
- DevOps: Docker + CI pipeline that runs tests and builds images.

---

## Key file references (high-signal)

- `GuhaStore.Web/Program.cs` — DI, DbContext, sessions, services registration.
- `GuhaStore.Infrastructure/Data/ApplicationDbContext.cs` — entity mappings and new DbSet registrations.
- `GuhaStore.Application/Services/*Service.cs` — where business logic lives (extend these services).
- `GuhaStore.Web/Controllers/*` and `GuhaStore.Web/Views/*` — controllers and Razor views to implement UI.

---

If this plan looks good I will convert the phase and tasks into tracked todos to start implementation.

### To-dos

- [ ] Prepare infra: admin layout, logging, config placeholders
- [ ] Implement Admin CRUD for Products, Orders, Categories, Brands, Customers and Dashboard
- [ ] Implement checkout flow with COD payment and order confirmation
- [ ] Add reviews, comments moderation and wishlist features
- [ ] Implement variant selector, advanced filters, responsive design and toasts
- [ ] Add Identity/role-based auth, caching, logging and unit/integration tests
- [ ] Dockerize app, add CI pipeline and deployment docs