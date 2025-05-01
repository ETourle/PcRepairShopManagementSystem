using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Store
{
    [Authorize(Roles = "Admin")]
    public class AddComponentModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public AddComponentModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, StringLength(100)]
            public string Name { get; set; }

            [Required, StringLength(50)]
            public string Category { get; set; }

            [Required, StringLength(500)]
            public string Description { get; set; }

            [Required, Range(0.01, 100000)]
            public decimal Price { get; set; }

            [Required, Range(0, int.MaxValue)]
            public int QuantityInStock { get; set; }

            [Required, StringLength(100)]
            public string Manufacturer { get; set; }

            [Required, Url]
            public string ImageUrl { get; set; }
        }

        public void OnGet()
        {
            // nothing needed; just render form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var component = new Component
            {
                Name = Input.Name,
                Category = Input.Category,
                Description = Input.Description,
                Price = Input.Price,
                QuantityInStock = Input.QuantityInStock,
                Manufacturer = Input.Manufacturer,
                ImageUrl = Input.ImageUrl
            };

            _dbContext.PCComponents.Add(component);
            await _dbContext.SaveChangesAsync();

            // Clear form and show a fresh page
            ModelState.Clear();
            ViewData["StatusMessage"] = "Component added successfully!";
            return Page();
        }
    }
}
