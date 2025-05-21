using System;
using System.IO;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;
using PcRepairShopManagementSystem.Areas.Services; // for GmailEmailSender
using PcRepairShopManagementSystem;                // for SynchronizedConverter, PdfTools

var builder = WebApplication.CreateBuilder(args);

// 1) DB & Identity (unchanged)
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Register IHttpClientFactory(for PDFrest)
builder.Services.AddHttpClient();

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 104857600; // 100MB
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// 2) Configure EmailSettings from configuration / usersecrets
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// 3) Register our SMTP email sender
builder.Services.AddTransient<IEmailSender, GmailEmailSender>();

var app = builder.Build();

// 6) Seed roles/admin (unchanged)
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentityDataInitializer.SeedData(userManager, roleManager);
}

// 7) Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    //app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// for Identity you must authenticate before you authorize
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.Run();
