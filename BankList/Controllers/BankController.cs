using System.Globalization;
using System.Text.Json;
using BankList.Helpers;
using BankList.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

//using System.Globalization;
//using CsvHelper.Configuration;
//using Microsoft.AspNetCore.Mvc;
//using BankList.Models;
//using System.IO;
//using System.Linq;
//using System.Collections.Generic;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
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


        //  JSON file read & respose JSON


        //[HttpGet("GetBank")]
        //public IActionResult GetBank(string ifsc)
        //{
        //    if (string.IsNullOrWhiteSpace(ifsc))                         //Checks if ifsc is null, empty,whitespace
        //        return BadRequest(new { message = "Please provide IFSC code." });

        //    var path = Path.Combine(_env.WebRootPath, "data", "banklists.json");

        //    if (!System.IO.File.Exists(path))                        // check file exits or not
        //        return NotFound(new { message = "JSON file not found." });

        //    var json = System.IO.File.ReadAllText(path);            //Reads the entire JSON file as a string

        //    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //    // PropertyNameCaseInsensitive = Matches JSON properties regardless of case.

        //    options.Converters.Add(new BankHelper());              //Custom converter to handle numbers as strings

        //    var banks = JsonSerializer.Deserialize<List<BankDetail>>(json, options);

        //    var bank = banks?.FirstOrDefault(b =>
        //        string.Equals(b.IFSC_CODE?.Trim(), ifsc.Trim(), StringComparison.OrdinalIgnoreCase));

        //    if (bank == null)
        //        return NotFound(new { message = "IFSC code not found." });

        //    // Return only BankName and BranchName
        //    //var result = new
        //    //{
        //    //    bank.BANK_NAME,
        //    //    bank.BRANCH_NAME
        //    //};
        //    // return Ok(result);

        //    return Ok(bank); // Returns JSON automatically
        //}




        //  CSV file read & respose Json

        [HttpGet("GetBank")]
        public IActionResult GetBank(string ifsc)
        {
            if (string.IsNullOrWhiteSpace(ifsc))
                return BadRequest(new { message = "Please provide IFSC code." });

            var path = Path.Combine(_env.WebRootPath, "data", "bnk.csv");

            if (!System.IO.File.Exists(path))
                return NotFound(new { message = "CSV file not found." });
          
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim
            };

            // Stream the CSV to avoid loading the entire file into memory
            using (var reader = new StreamReader(path))
            using (var csv = new CsvHelper.CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    var bank = csv.GetRecord<BankDetail>();
                    if (bank.IFSC_CODE.Equals(ifsc, StringComparison.OrdinalIgnoreCase))
                    {
                        return Ok(bank); // Return JSON as soon as IFSC is found
                    }
                }
            }

            return NotFound(new { message = "IFSC code not found." });
        }





    }
}
