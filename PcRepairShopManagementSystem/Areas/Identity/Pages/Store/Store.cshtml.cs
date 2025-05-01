using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Data;
using PcRepairShopManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Store
{
    public class StoreModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private const int PageSize = 10;

        // 1) Hard-coded filter options:
        private static readonly string[] AllowedCategories =
            { "Graphics Card", "Processor", "SSD", "HDD", "Memory" };
        private static readonly string[] AllowedBrands =
            { "AMD", "NVIDIA", "CORSAIR", "MSI", "CRUCIAL" };

        public StoreModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<Component> Components { get; private set; }
        public IList<string> Categories { get; private set; }
        public IList<string> Brands { get; private set; }
        public int PageNumber { get; private set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage { get; private set; }
        public string SortOrder { get; private set; }
        public string CategoryFilter { get; private set; }
        public string BrandFilter { get; private set; }

        public async Task OnGetAsync(
            string sortOrder,
            string categoryFilter,
            string brandFilter,
            int pageNumber = 1)
        {
            SortOrder = sortOrder;
            CategoryFilter = categoryFilter;
            BrandFilter = brandFilter;
            PageNumber = pageNumber;

            // 2) Use the static arrays for dropdowns:
            Categories = AllowedCategories.ToList();
            Brands = AllowedBrands.ToList();

            // 3) Base query
            var query = _dbContext.PCComponents.AsNoTracking();

            // 4) Apply hard-coded filters only if they match:
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

            // 5) Sorting
            query = sortOrder switch
            {
                "price_desc" => query.OrderByDescending(c => c.Price),
                _ => query.OrderBy(c => c.Price),
            };

            // 6) Pagination
            var totalCount = await query.CountAsync();
            Components = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            HasNextPage = PageNumber * PageSize < totalCount;
        }
    }
}
