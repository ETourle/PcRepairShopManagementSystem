using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Store
{
    public class CartModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public CartModel(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Nothing needed on GET; JS builds the UI
        public void OnGet() { }

        // Accepts JSON array of CartItem
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostCheckoutAsync([FromBody] List<CartItem> cart)
        {
            if (cart == null || !cart.Any())
                return BadRequest();

            // 1. Identify customer
            var aspUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (aspUserId == null) return Unauthorized();

            var customer = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.ApplicationUserId == aspUserId);
            if (customer == null)
                return BadRequest("Complete your customer profile first.");

            // 2. Build Order + details
            var order = new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.Sum(i => i.UnitPrice * i.Quantity),
                IsShipped = false,
                IsForDelivery = false,
                OrderDetails = cart.Select(i => new OrderDetail
                {
                    ComponentId = i.ComponentId,
                    ComponentName = i.ComponentName,
                    Quantity = i.Quantity,
                    PricePerUnit = i.UnitPrice
                }).ToList()
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            return new OkResult();
        }

        // DTO for checkout
        public class CartItem
        {
            public int ComponentId { get; set; }
            public string ComponentName { get; set; }
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
        }
    }
}
