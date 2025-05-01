using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Store
{
    public class CheckoutModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public CheckoutModel(
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required] public string ShippingType { get; set; }
            [Required] public string CartData { get; set; }

            // Delivery fields
            [Required, StringLength(200)] public string StreetAddress1 { get; set; }
            [Required, StringLength(200)] public string StreetAddress2 { get; set; }
            [Required, StringLength(100)] public string City { get; set; }
            [Required, StringLength(100)] public string StateOrProvince { get; set; }
            [Required, StringLength(20)] public string PostalCode { get; set; }

            // Collection only
            public bool PayAtStore { get; set; }
        }

        private class CartItem
        {
            public int ComponentId { get; set; }
            public string ComponentName { get; set; }
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
        }

        public void OnGet() { /* no-op */ }

        public Task<IActionResult> OnPostDeliveryAsync()
            => ProcessOrderAsync(isDelivery: true);

        public Task<IActionResult> OnPostCollectionAsync()
            => ProcessOrderAsync(isDelivery: false);

        private async Task<IActionResult> ProcessOrderAsync(bool isDelivery)
        {
            if (!ModelState.IsValid)
                return Page();

            // 1) Deserialize cart (camelCase → PascalCase)
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer
                .Deserialize<List<CartItem>>(Input.CartData ?? "[]", opts)
                ?? new List<CartItem>();

            if (items.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return Page();
            }

            // 2) Find the customer
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var customer = await _dbContext.Customers
                .SingleOrDefaultAsync(c => c.ApplicationUserId == user.Id);
            if (customer == null)
            {
                ModelState.AddModelError("", "Customer profile not found.");
                return Page();
            }

            // 3) Build Order shell
            var order = new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.UtcNow,
                IsForDelivery = isDelivery,
                IsPaid = !isDelivery,
                StreetAddress1 = isDelivery ? Input.StreetAddress1 : null,
                StreetAddress2 = isDelivery ? Input.StreetAddress2 : null,
                City = isDelivery ? Input.City : null,
                StateOrProvince = isDelivery ? Input.StateOrProvince : null,
                PostalCode = isDelivery ? Input.PostalCode : null,
                OrderDetails = new List<OrderDetail>()
            };

            // 4) Fetch components in one go
            var compIds = items.Select(i => i.ComponentId).Distinct().ToList();
            var dbComps = await _dbContext.PCComponents
                .Where(c => compIds.Contains(c.Id))
                .ToListAsync();
            var compMap = dbComps.ToDictionary(c => c.Id);

            // 5) Populate details + adjust stock + total
            decimal total = 0;
            foreach (var ci in items)
            {
                total += ci.UnitPrice * ci.Quantity;

                order.OrderDetails.Add(new OrderDetail
                {
                    ComponentId = ci.ComponentId,
                    ComponentName = compMap.TryGetValue(ci.ComponentId, out var dbC)
                                        ? dbC.Name
                                        : $"Component #{ci.ComponentId}",
                    PricePerUnit = ci.UnitPrice,
                    Quantity = ci.Quantity
                });

                // decrement stock
                if (compMap.TryGetValue(ci.ComponentId, out var cItem))
                    cItem.QuantityInStock = Math.Max(0, cItem.QuantityInStock - ci.Quantity);
            }
            order.TotalAmount = total;

            // 6) Save everything
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            // 7) Send confirmation email
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<h2>Thank you for your order #{order.Id}</h2>");
            sb.AppendLine("<ul>");
            foreach (var d in order.OrderDetails)
            {
                sb.AppendLine($"<li>{d.ComponentName} × {d.Quantity} @ {d.PricePerUnit:C}</li>");
            }
            sb.AppendLine("</ul>");
            sb.AppendLine($"<p><strong>Total: {order.TotalAmount:C}</strong></p>");
            if (isDelivery)
            {
                sb.AppendLine("<p>Shipping to:</p>");
                sb.AppendLine($"<p>{order.StreetAddress1}<br/>{order.StreetAddress2}<br/>{order.City}, {order.StateOrProvince} {order.PostalCode}</p>");
            }
            else
            {
                sb.AppendLine("<p>Collection chosen.  Please pay at store.</p>");
            }

            await _emailSender.SendEmailAsync(
                user.Email,
                $"Your PC Repair Hub Order Confirmation (#{order.Id})",
                sb.ToString()
            );

            // 8) Redirect to confirmation page
            return RedirectToPage("/Store/OrderConfirmation", new { id = order.Id });
        }
    }
}
