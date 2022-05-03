using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WEB_Shop_Ajax.Data;
using WEB_Shop_Ajax.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WEB_Shop_AjaxContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WEB_Shop_AjaxContext")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<WEB_Shop_AjaxContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options => //CookieAuthenticationOptions
        {
            options.LoginPath = new PathString("/Account/Login");
            options.Cookie.Expiration = TimeSpan.FromDays(3650);
        });

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
