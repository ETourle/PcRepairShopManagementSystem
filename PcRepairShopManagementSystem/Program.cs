using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;
using PcRepairShopManagementSystem.Areas.Services; // <-- import for IEmailSender/SmtpEmailSender

var builder = WebApplication.CreateBuilder(args);

// 1) DB & Identity (unchanged)
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// 2) Configure EmailSettings from configuration / user?secrets
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// 3) Register our SMTP email sender
builder.Services.AddTransient<IEmailSender, GmailEmailSender>();

var app = builder.Build();

// 4) Seed roles/admin (unchanged)
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentityDataInitializer.SeedData(userManager, roleManager);
}

// 5) Middleware (unchanged)
if (app.Environment.IsDevelopment())
    app.UseMigrationsEndPoint();
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
