using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Store
{
    [Authorize(Roles = "Admin")]
    public class StaffOrderHubModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;

        public StaffOrderHubModel(ApplicationDbContext dbContext, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        public IList<Order> Orders { get; set; }

        public async Task OnGetAsync()
        {
            // load *all* orders, shipped or not
            Orders = await _dbContext.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.ApplicationUser)
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostMarkShippedAsync(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.ApplicationUser)
                .Include(o => o.OrderDetails)
                .SingleOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            order.IsShipped = true;
            await _dbContext.SaveChangesAsync();

            // send notification email
            var sb = new StringBuilder();
            sb.AppendLine($"Hello {order.Customer.FirstName},");
            sb.AppendLine();
            sb.AppendLine($"Your order #{order.Id} has now shipped:");
            sb.AppendLine();
            foreach (var d in order.OrderDetails)
                sb.AppendLine($"• {d.ComponentName} × {d.Quantity} @ {d.PricePerUnit:C}");
            sb.AppendLine();
            sb.AppendLine("Thank you for choosing PC Repair Hub!");

            await _emailSender.SendEmailAsync(
                order.Customer.ApplicationUser.Email,
                $"Order #{order.Id} Shipped!",
                sb.ToString()
            );

            return RedirectToPage();  // refresh in place
        }
    }
}
