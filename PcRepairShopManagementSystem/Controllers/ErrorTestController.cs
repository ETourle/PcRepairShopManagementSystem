using Microsoft.AspNetCore.Mvc;

namespace PcRepairShopManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorTestController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            return Content("Backend diagnostic message from ErrorTestController.", "text/plain", System.Text.Encoding.UTF8);
        }
    }
}