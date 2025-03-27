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
    public class GrabAppointmentsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public GrabAppointmentsModel(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IList<Appointment> Appointments { get; set; }

        public async Task OnGetAsync()
        {
            // Load unassigned appointments, including Customer and the linked IdentityUser for email.
            Appointments = await _dbContext.Appointments
                .Include(a => a.Customer)
                    .ThenInclude(c => c.ApplicationUser)
                .Where(a => a.StaffId == null)
                .OrderBy(a => a.StartDate)
                .ToListAsync();
        }

        // Handler to accept an appointment
        public async Task<IActionResult> OnPostAcceptAsync(int id)
        {
            // Get the current logged-in user ID.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User not found.");
            }

            // Find the corresponding staff record for this user using ApplicationUserId.
            var staff = await _dbContext.Staff.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (staff == null)
            {
                return NotFound("Staff record not found. Please complete your staff details.");
            }

            // Find the appointment by its ID.
            var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            // Assign the current staff's id to the appointment and update its status.
            appointment.StaffId = staff.Id;
            appointment.Status = "Accepted";

            await _dbContext.SaveChangesAsync();

            // Redirect back to the same page to refresh the list.
            return RedirectToPage();
        }
    }
}
