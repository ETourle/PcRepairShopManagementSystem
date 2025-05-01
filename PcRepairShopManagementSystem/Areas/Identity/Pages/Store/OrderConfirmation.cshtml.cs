using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Store
{
    public class OrderConfirmationModel : PageModel
    {
        public int OrderId { get; private set; }
        public void OnGet(int id) => OrderId = id;
    }
}
