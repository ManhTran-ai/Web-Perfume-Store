using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;
using GuhaStore.Infrastructure.Repositories;
using GuhaStore.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using MySql.EntityFrameworkCore.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/guhastore-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Use Serilog as the logging provider
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        "Server=localhost;Database=dbperfume_simple;User=root;Password=;CharSet=utf8mb4;"));

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Caching
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

// Add Health Checks
builder.Services.AddHealthChecks();

// Register UnitOfWork and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Application Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IFileUploadService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new FileUploadService(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
});
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

// Register Payment Services
builder.Services.AddScoped<VnPayService>();
builder.Services.AddScoped<MoMoService>();

// Add HttpContextAccessor for CartService
builder.Services.AddHttpContextAccessor();

// Add Authorization with policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
        {
            var accountType = context.User.FindFirst("AccountType")?.Value;
            return accountType == "2"; // Admin
        }));

    options.AddPolicy("StaffOrAdmin", policy =>
        policy.RequireAssertion(context =>
        {
            var accountType = context.User.FindFirst("AccountType")?.Value;
            return accountType == "1" || accountType == "2"; // Staff or Admin
        }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Session middleware
app.UseSession();

// Add custom error handling middleware
app.UseMiddleware<GuhaStore.Web.Middleware.DatabaseErrorHandlerMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Admin area routes
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" });

app.MapControllerRoute(
    name: "adminArea",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Health check endpoint
app.MapHealthChecks("/health");

app.Run();
