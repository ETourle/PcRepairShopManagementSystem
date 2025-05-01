using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Store
{
    public class StockManagementModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private const int PageSize = 10;

        // same hard-coded filters
        private static readonly string[] AllowedCategories =
            { "Graphics Card", "Processor", "SSD", "HDD", "Memory" };
        private static readonly string[] AllowedBrands =
            { "AMD", "NVIDIA", "CORSAIR", "MSI", "CRUCIAL" };

        public StockManagementModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<Component> Components { get; private set; }
        public IList<string> Categories { get; private set; }
        public IList<string> Brands { get; private set; }

        public int PageNumber { get; private set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage { get; private set; }

        // filter & sort state
        [BindProperty(SupportsGet = true)]
        public string CategoryFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public string BrandFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortStock { get; set; } // "stock" or "stock_desc"

        public async Task OnGetAsync(
            int pageNumber = 1,
            string categoryFilter = "",
            string brandFilter = "",
            string sortStock = "stock")
        {
            PageNumber = pageNumber;
            CategoryFilter = categoryFilter;
            BrandFilter = brandFilter;
            SortStock = sortStock;

            Categories = AllowedCategories.ToList();
            Brands = AllowedBrands.ToList();

            var query = _dbContext.PCComponents
                .AsNoTracking();

            // apply filters
            if (!string.IsNullOrEmpty(CategoryFilter)
                && AllowedCategories.Contains(CategoryFilter))
            {
                query = query.Where(c => c.Category == CategoryFilter);
            }
            if (!string.IsNullOrEmpty(BrandFilter)
                && AllowedBrands.Contains(BrandFilter))
            {
                query = query.Where(c => c.Manufacturer == BrandFilter);
            }

            // apply stock sort
            query = SortStock switch
            {
                "stock_desc" => query.OrderByDescending(c => c.QuantityInStock),
                _ => query.OrderBy(c => c.QuantityInStock),
            };

            // pagination
            var total = await query.CountAsync();
            Components = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            HasNextPage = PageNumber * PageSize < total;
        }

        public async Task<IActionResult> OnPostIncreaseAsync(
            int id,
            int pageNumber,
            string categoryFilter,
            string brandFilter,
            string sortStock)
        {
            var comp = await _dbContext.PCComponents.FindAsync(id);
            if (comp != null)
            {
                comp.QuantityInStock++;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToPage(new
            {
                pageNumber,
                categoryFilter,
                brandFilter,
                sortStock
            });
        }

        public async Task<IActionResult> OnPostDecreaseAsync(
            int id,
            int pageNumber,
            string categoryFilter,
            string brandFilter,
            string sortStock)
        {
            var comp = await _dbContext.PCComponents.FindAsync(id);
            if (comp != null && comp.QuantityInStock > 0)
            {
                comp.QuantityInStock--;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToPage(new
            {
                pageNumber,
                categoryFilter,
                brandFilter,
                sortStock
            });
        }
    }
}
