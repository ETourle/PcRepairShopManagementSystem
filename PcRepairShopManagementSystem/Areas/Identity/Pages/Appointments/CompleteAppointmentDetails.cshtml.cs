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
    public class CompleteAppointmentDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public CompleteAppointmentDetailsModel(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public int AppointmentId { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Title")]
            public string Title { get; set; }

            [Required]
            [Display(Name = "Category")]
            public string Category { get; set; }

            [Display(Name = "Software Title")]
            public string SoftwareTitle { get; set; }

            [Display(Name = "Brand")]
            public string Brand { get; set; }

            [Display(Name = "Model")]
            public string Model { get; set; }

            [Display(Name = "Serial Number")]
            public string SerialNumber { get; set; }

            [Display(Name = "Additional Details")]
            public string AdditionalNotes { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            AppointmentId = id;

            // Retrieve the appointment by id.
            var appointment = await _dbContext.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            // Check that the appointment is accepted (i.e. StaffId is assigned).
            if (appointment.StaffId == null)
            {
                return BadRequest("Appointment is not yet accepted by a staff member.");
            }

            // Pre-fill the title with the appointment's IssueTitle.
            Input = new InputModel
            {
                Title = appointment.IssueTitle
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            AppointmentId = id;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve the appointment by id.
            var appointment = await _dbContext.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            // Retrieve or create the AppointmentDetail record (one-to-one relationship using AppointmentId as PK).
            var detail = await _dbContext.AppointmentDetails.FindAsync(id);
            if (detail == null)
            {
                detail = new AppointmentDetail
                {
                    AppointmentId = id,
                    Title = Input.Title,
                    Category = Input.Category,
                    SoftwareTitle = Input.SoftwareTitle,
                    Brand = Input.Brand,
                    Model = Input.Model,
                    SerialNumber = Input.SerialNumber,
                    AdditionalNotes = Input.AdditionalNotes
                };
                _dbContext.AppointmentDetails.Add(detail);
            }
            else
            {
                detail.Title = Input.Title;
                detail.Category = Input.Category;
                detail.SoftwareTitle = Input.SoftwareTitle;
                detail.Brand = Input.Brand;
                detail.Model = Input.Model;
                detail.SerialNumber = Input.SerialNumber;
                detail.AdditionalNotes = Input.AdditionalNotes;
            }

            // Update the appointment status to "Completed" and optionally set the CompletionDate.
            appointment.Status = "Completed";
            

            await _dbContext.SaveChangesAsync();

            // Redirect to the Appointment Hub page.
            return RedirectToPage("./AppointmentsHub");
        }
    }
}
