using GoodHake.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// In-Memory-Datenbank hinzufügen!
builder.Services.AddDbContext<GDDBContext>(options =>
{
    options.UseInMemoryDatabase("CalorieDB"); // Hier wird die In-Memory-DB konfiguriert!
});

// Authentication & Sessions
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; // Redirect, wenn nicht eingeloggt
    });

builder.Services.AddSession();

var app = builder.Build();

// `Seed()` sicher aufrufen!
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GDDBContext>();

    Console.WriteLine("Seeding-Datenbank...");
    context.Seed(); // User "root" anlegen!
    Console.WriteLine("Seeding abgeschlossen.");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
