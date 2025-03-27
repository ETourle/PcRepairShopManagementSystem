using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Appointments
{
    public class BookAppointmentModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public BookAppointmentModel(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Preferred Appointment Date & Time")]
            public DateTime StartDate { get; set; }

            [Required]
            [Display(Name = "Issue Title")]
            public string IssueTitle { get; set; }

            
        }

        public void OnGet()
        {
            ViewData["ActivePage"] = "BookAppointment";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve the current user's ID from the Identity claims.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User not found.");
            }

            // Retrieve the Customer record for the current user.
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
            if (customer == null)
            {
                // Redirect the user if their account details haven't been completed.
                return NotFound("Customer details not found. Please complete your account details first.");
            }

            // Create a new Appointment record with basic details.
            // StaffId remains null until a staff member accepts the appointment.
            var appointment = new Appointment
            {
                CustomerId = customer.Id,
                StaffId = null, // Appointment is unassigned initially.
                StartDate = Input.StartDate,
                CompletionDate = null, // Now nullable; will be set upon completion.
                IssueTitle = Input.IssueTitle,
                Status = "Pending" // Initial status.
            };

            _dbContext.Appointments.Add(appointment);
            await _dbContext.SaveChangesAsync();

            // Redirect to a confirmation page (or you can display a status message)
            return RedirectToPage("/Appointments/ConfirmAppointment");
        }
    }
}
