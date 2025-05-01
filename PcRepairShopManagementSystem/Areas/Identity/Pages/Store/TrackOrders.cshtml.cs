using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Store
{
    public class TrackOrdersModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public TrackOrdersModel(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IList<Order> Orders { get; private set; }

        public async Task OnGetAsync()
        {
            // 1) identify the current user and their Customer record
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                Orders = new List<Order>();
                return;
            }

            var customer = await _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
            {
                Orders = new List<Order>();
                return;
            }

            // 2) load all their orders with details
            Orders = await _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.OrderDetails)
                .Where(o => o.CustomerId == customer.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
