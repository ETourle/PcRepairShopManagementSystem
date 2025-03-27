using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PcRepairShopManagementSystem.Areas.Identity.Pages.Appointments
{
    public class ConfirmAppointmentModel : PageModel
    {
        public void OnGet()
        {
            // Set the active page for navigation purposes
            ViewData["ActivePage"] = "ConfirmAppointment";

            
        }
    }
}
