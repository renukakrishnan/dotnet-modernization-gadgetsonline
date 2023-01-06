
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GadgetsOnline.Models;
using GadgetsOnline.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();
ConfigureServices(builder.Services);
var app = builder.Build();

Configure();
app.Run();

void ConfigureServices(IServiceCollection services)
{
    services.AddDistributedMemoryCache();
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(1);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    services.AddControllersWithViews();
    services.AddDbContext<GadgetsOnlineEntities>(opt =>
    {

        opt.UseSqlServer(builder.Configuration.GetConnectionString(nameof(GadgetsOnlineEntities)));
    });

    // seed data
    using (var context = new GadgetsOnlineEntities(builder.Configuration.GetConnectionString(nameof(GadgetsOnlineEntities))))
    {
        context.Database.EnsureCreated();
        context.SaveChanges();
    }

    services.AddScoped<IInventory, Inventory>();
    services.AddScoped<IShoppingCart, ShoppingCart>();
    services.AddScoped<IOrderProcessing, OrderProcessing>();
    //Added Services
}

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
void Configure()
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    //Added Middleware

    app.UseRouting();

    app.UseAuthorization();

    app.UseSession();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });
}
