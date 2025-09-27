using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace BankList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostalController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public PostalController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByPincode([FromQuery] string pincode)
        {
            if (string.IsNullOrWhiteSpace(pincode))
                return BadRequest(new { message = "Please provide a valid pincode." });

            using var httpClient = new HttpClient();
            string url = $"https://api.postalpincode.in/pincode/{pincode}";
            var response = await httpClient.GetStringAsync(url);

            var json = JsonDocument.Parse(response).RootElement[0];

            if (json.GetProperty("Status").GetString() != "Success")
                return NotFound(new { message = "No data found for this pincode." });

            return Ok(new
            {
                Message = json.GetProperty("Message").GetString(),
                PostOffices = json.GetProperty("PostOffice")
            });
        }
    }
}
