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

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Appointments
{
    public class StaffAppointmentHubModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public StaffAppointmentHubModel(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IList<Appointment> Appointments { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            var staff = await _dbContext.Staff
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (staff == null) return NotFound("Staff record not found.");

            Appointments = await _dbContext.Appointments
                .Include(a => a.Customer).ThenInclude(c => c.ApplicationUser)
                .Include(a => a.AppointmentDetail)
                .Where(a => a.StaffId == staff.Id)
                .OrderByDescending(a => a.StartDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            var appt = await _dbContext.Appointments.FindAsync(id);
            if (appt == null) return NotFound();

            appt.Status = "Completed";
            appt.CompletionDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
