using System.Text.Json;
using BankList.Helpers;
using BankList.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankList.Controllers
{
    [Route("Bank")]
    [ApiController] // Important for JSON responses
    public class BankController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public BankController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("GetBank")]
        public IActionResult GetBank(string ifsc)
        {
            if (string.IsNullOrWhiteSpace(ifsc))    //Checks if ifsc is null, empty,whitespace
                return BadRequest(new { message = "Please provide IFSC code." });

            var path = Path.Combine(_env.WebRootPath, "data", "banklists.json");

            if (!System.IO.File.Exists(path))   // check file exits or not
                return NotFound(new { message = "JSON file not found." });

            var json = System.IO.File.ReadAllText(path); //Reads the entire JSON file as a string

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; //PropertyNameCaseInsensitive = Matches JSON properties regardless of case.

            options.Converters.Add(new BankHelper());  //Custom converter to handle numbers as strings

            var banks = JsonSerializer.Deserialize<List<BankDetail>>(json, options);

            var bank = banks?.FirstOrDefault(b =>
                string.Equals(b.IFSC_CODE?.Trim(), ifsc.Trim(), StringComparison.OrdinalIgnoreCase));

            if (bank == null)
                return NotFound(new { message = "IFSC code not found." });

            // Return only BankName and BranchName

            //var result = new
            //{
            //    bank.BANK_NAME,
            //    bank.BRANCH_NAME
            //};
            //return Ok(result);

            return Ok(bank); // Returns JSON automatically
        }

    }
}
