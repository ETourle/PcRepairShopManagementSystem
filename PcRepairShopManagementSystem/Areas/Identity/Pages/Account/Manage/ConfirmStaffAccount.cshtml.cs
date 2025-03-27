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

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Account.Manage
{
    public class ConfirmStaffAccountModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmStaffAccountModel(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }

            [Required]
            [Display(Name = "Office Phone")]
            public string OfficePhone { get; set; }

            [Required]
            [Display(Name = "Hire Date")]
            [DataType(DataType.Date)]
            public DateTime HireDate { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ActivePage"] = "ConfirmStaffAccount";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the current user's ID from the Identity claims.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User not found.");
            }


            // Retrieve the Staff record by matching ApplicationUserId (not the primary key)
            var staff = await _dbContext.Staff.FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (staff == null)
            {
                // If no record exists, create a new Staff record.
                staff = new Staff
                {
                    ApplicationUserId = userId,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    JobTitle = Input.JobTitle,
                    OfficePhone = Input.OfficePhone,
                    HireDate = Input.HireDate
                };
                _dbContext.Staff.Add(staff);
            }
            else
            {
                // If the record exists, update the staff details.
                staff.FirstName = Input.FirstName;
                staff.LastName = Input.LastName;
                staff.JobTitle = Input.JobTitle;
                staff.OfficePhone = Input.OfficePhone;
                staff.HireDate = Input.HireDate;
            }

            await _dbContext.SaveChangesAsync();

            // Redirect to the manage index page or a confirmation page.
            return RedirectToPage("./Index");
        }
    }
}
