using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PcRepairShopManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PdfController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PdfRequestModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Html))
                return BadRequest("Missing or empty HTML in request.");

            var apiKey = "eca391b7-d1df-48f2-9596-32dc1393c17e";
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Api-Key", apiKey);

            // Do not HtmlEncode, just serialize!
            var payload = new { html = model.Html };
            var apiRequestContent = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("https://api.pdfrest.com/html-pdf", apiRequestContent);

            if (!response.IsSuccessStatusCode)
            {
                var resp = await response.Content.ReadAsStringAsync();
                return StatusCode(400, $"PDFrest API call failed ({response.StatusCode}): {resp}");
            }

            var pdfBytes = await response.Content.ReadAsByteArrayAsync();
            return File(pdfBytes, "application/pdf", "PrintTest.pdf");
        }
    }

    public class PdfRequestModel
    {
        public string Html { get; set; }
    }
}
