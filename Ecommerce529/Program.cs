using Ecommerce529.LifeTime.ClassLifeTime;
using Ecommerce529.LifeTime.InterfaceLifeTime;
using Ecommerce529.Repositories;
using Ecommerce529.Utilities.DbInitializer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Ecommerce529
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string"
                    + "'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(connectionString);
            });
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                //options.Lockout.DefaultLockoutTimeSpan = ;
                //options.Lockout.MaxFailedAccessAttempts = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Register
            // AddTransient  , AddScoped    ,  AddSingleton
            builder.Services.AddScoped<IRepository<Product> , Repository<Product>>();
            builder.Services.AddScoped<IRepository<Category> , Repository<Category>>(); 
            builder.Services.AddScoped<IRepository<ApplicationUserOtp> , Repository<ApplicationUserOtp>>(); 
            builder.Services.AddScoped<IRepository<Brand> , Repository<Brand>>(); 
            builder.Services.AddScoped<IRepository<Cart> , Repository<Cart>>(); 
            builder.Services.AddScoped<IRepository<Promotion> , Repository<Promotion>>(); 
            builder.Services.AddScoped<IProductColorRepository , ProductColorRepository>(); 
            builder.Services.AddScoped<IProductSubImageRepository , ProductSubImageRepository>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<IDbInitializer, DbInitializer>();




            // test LifeTime
            builder.Services.AddTransient<ITransientInterface, TransientClass>(); 
            builder.Services.AddScoped<IScopedInterface ,ScopedClass>(); 
            builder.Services.AddSingleton<ISingletonInterface ,SingletonClass>();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await dbInitializer.InitializeAsync();
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
