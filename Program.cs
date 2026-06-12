using Food_Market_BE.Modules.AuthModule.Helpers;
using Food_Market_BE.Modules.AuthModule.Repositories.Implementations;
using Food_Market_BE.Modules.AuthModule.Repositories.Interfaces;
using Food_Market_BE.Modules.AuthModule.Services.Implementations;
using Food_Market_BE.Modules.AuthModule.Services.Interfaces;
using Food_Market_BE.Modules.CartModule.Repositories.Implementations;
using Food_Market_BE.Modules.CartModule.Repositories.Interfaces;
using Food_Market_BE.Modules.CartModule.Services.Implementations;
using Food_Market_BE.Modules.CartModule.Services.Interfaces;
using Food_Market_BE.Modules.CategoryModule.Repositories.Implementations;
using Food_Market_BE.Modules.CategoryModule.Repositories.Interfaces;
using Food_Market_BE.Modules.CategoryModule.Services.Implementations;
using Food_Market_BE.Modules.CategoryModule.Services.Interfaces;
using Food_Market_BE.Modules.FavoriteModule.Repositories.Implementations;
using Food_Market_BE.Modules.FavoriteModule.Repositories.Interfaces;
using Food_Market_BE.Modules.FavoriteModule.Services.Implementations;
using Food_Market_BE.Modules.FavoriteModule.Services.Interfaces;
using Food_Market_BE.Modules.OrderModule.Repositories.Implementations;
using Food_Market_BE.Modules.OrderModule.Repositories.Interfaces;
using Food_Market_BE.Modules.OrderModule.Services.Implementations;
using Food_Market_BE.Modules.OrderModule.Services.Interfaces;
using Food_Market_BE.Modules.ProductModule.Repositories.Implementations;
using Food_Market_BE.Modules.ProductModule.Repositories.Interfaces;
using Food_Market_BE.Modules.ProductModule.Services.Implementations;
using Food_Market_BE.Modules.ProductModule.Services.Interfaces;
using Food_Market_BE.Modules.ReviewModule.Repositories.Implementations;
using Food_Market_BE.Modules.ReviewModule.Repositories.Interfaces;
using Food_Market_BE.Modules.ReviewModule.Services.Implementations;
using Food_Market_BE.Modules.ReviewModule.Services.Interfaces;
using Food_Market_BE.Modules.SellerModule.Repositories.Implementations;
using Food_Market_BE.Modules.SellerModule.Repositories.Interfaces;
using Food_Market_BE.Modules.SellerModule.Services.Implementations;
using Food_Market_BE.Modules.SellerModule.Services.Interfaces;
using Food_Market_BE.Modules.StoreModule.Repositories.Implementations;
using Food_Market_BE.Modules.StoreModule.Repositories.Interfaces;
using Food_Market_BE.Modules.StoreModule.Services.Implementations;
using Food_Market_BE.Modules.StoreModule.Services.Interfaces;
using Food_Market_BE.Modules.UserProfileModule.Repositories.Implementations;
using Food_Market_BE.Modules.UserProfileModule.Repositories.Interfaces;
using Food_Market_BE.Modules.UserProfileModule.Services.Implementations;
using Food_Market_BE.Modules.UserProfileModule.Services.Implemetations;
using Food_Market_BE.Modules.UserProfileModule.Services.Interfaces;
using Food_Market_BE.Modules.VoucherModule.Repositories.Implementations;
using Food_Market_BE.Modules.VoucherModule.Repositories.Interfaces;
using Food_Market_BE.Modules.VoucherModule.Services.Implementations;
using Food_Market_BE.Modules.VoucherModule.Services.Interfaces;
using Food_Market_BE.Shared.Database;
using Food_Market_BE.Shared.Extensions;
using Food_Market_BE.Shared.Middleware;
using Food_Market_BE.Shared.Seeder.DataSeeder;
using Food_Market_BE.Shared.Seeder.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Mongo builder
builder.Services.AddSingleton<MongoDbContext>();

// Auth
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<TokenGenerator>();

// User Profile
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IFileUpDelService, FileUpDelService>();

// Store
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IStoreService, StoreService>();

// Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// Category
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Cart
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

// Order
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Review
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Favorite
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

// Voucher
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IVoucherService, VoucherService>();

// Seller
builder.Services.AddScoped<ISellerService, SellerService>();
builder.Services.AddScoped<ISellerRepository, SellerRepository>();

// Seeder
builder.Services.AddScoped<IDataSeeder, CategorySeeder>();
builder.Services.AddScoped<IDataSeeder, VoucherSeeder>();
builder.Services.AddScoped<IDataSeeder, StoreSeeder>();
builder.Services.AddScoped<IDataSeeder, ProductSeeder>();
builder.Services.AddScoped<IDataSeeder, CartSeeder>();
builder.Services.AddScoped<IDataSeeder, FavoriteSeeder>();
builder.Services.AddScoped<IDataSeeder, OrderSeeder>();
builder.Services.AddScoped<IDataSeeder, ReviewSeeder>();

// Others

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFE", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000"  // FE dev HTTP
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCustomMiddleware();

// Add routing and CORS
app.UseRouting();

app.UseCors("AllowFE");

// Add uploaded files static access
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
