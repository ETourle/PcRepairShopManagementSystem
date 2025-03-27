using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Appointments
{
    public class AppointmentHubModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentHubModel(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IList<Appointment> Appointments { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the current Identity user's ID.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User not found.");
            }

            // Retrieve the Customer record for the logged-in user using ApplicationUserId.
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
            if (customer == null)
            {
                return NotFound("Customer details not found. Please complete your account details first.");
            }

            // Load all appointments for this customer, including the linked Identity user (for email)
            Appointments = await _dbContext.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.ApplicationUser)
                .Where(a => a.CustomerId == customer.Id)
                .OrderByDescending(a => a.StartDate)
                .ToListAsync();

            return Page();
        }
    }
}
