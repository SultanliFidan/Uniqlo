using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Uniqlol.DataAccess;
using Uniqlol.Extensions;
using Uniqlol.Models;

namespace Uniqlol
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<UniqloDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSql"));
            });


            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = false;
                opt.Password.RequiredLength = 3;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<UniqloDbContext>();
            builder.Services.AddSession();
            builder.Services.ConfigureApplicationCookie(x =>
            {
                x.LoginPath = "/login";
                x.AccessDeniedPath = "/Home/AccessDenied";
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
            

            app.UseAuthorization();
            app.UseUserSeed();
            app.UseSession();

            //app.MapAreaControllerRoute("area", "Admin", "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "login",
                pattern: "login", new
                {
                    Controller = "Account",
                    Action = "Login"
                });
            app.MapControllerRoute(
               name: "register",
               pattern: "register", new
               {
                   Controller = "Account",
                   Action = "Register"
               });
            app.MapControllerRoute(
                name: "area",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            

            app.Run();
        }
    }
}
