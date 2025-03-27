using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Account.Manage
{
    public class ConfirmAccountModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmAccountModel(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
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
            [Display(Name = "Street Address")]
            public string StreetAddress { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            [Display(Name = "State/Province")]
            public string StateOrProvince { get; set; }

            [Required]
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            [Required]
            public string Country { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["ActivePage"] = "ConfirmAccount";
            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // If the user is in the "Admin" role, redirect them to the staff-specific page.
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToPage("./ConfirmStaffAccount");
            }

            // Otherwise, continue displaying the customer confirm account page.
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve the current user's ID from the Identity system
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User not found.");
            }

            // Check if a Customer record already exists for this user
            // For a one-to-one relationship, assume there's only one record per user.
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
            if (customer == null)
            {
                // Create a new Customer record linked to the Identity user
                customer = new Customer
                {
                    ApplicationUserId = userId,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    StreetAddress = Input.StreetAddress,
                    City = Input.City,
                    StateOrProvince = Input.StateOrProvince,
                    PostalCode = Input.PostalCode,
                    Country = Input.Country,
                    CreatedDate = DateTime.UtcNow
                };
                _dbContext.Customers.Add(customer);
            }
            else
            {
                // Update existing Customer record
                customer.FirstName = Input.FirstName;
                customer.LastName = Input.LastName;
                customer.StreetAddress = Input.StreetAddress;
                customer.City = Input.City;
                customer.StateOrProvince = Input.StateOrProvince;
                customer.PostalCode = Input.PostalCode;
                customer.Country = Input.Country;
            }

            await _dbContext.SaveChangesAsync();

            // Redirect to a profile or confirmation page after saving details
            return RedirectToPage("./Index");
        }
    }
}
