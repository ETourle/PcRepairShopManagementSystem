using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PcRepairShopManagementSystem.Pages
{
    public class TestErrorModel : PageModel
    {
        [IgnoreAntiforgeryToken]
        public IActionResult OnPost()
        {
            return new ContentResult
            {
                StatusCode = 400,
                Content = "Backend diagnostic message from TestError page.",
                ContentType = "text/plain"
            };
        }
    }
}
