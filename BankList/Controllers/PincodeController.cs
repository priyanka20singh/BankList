using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using BankList.Models;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;

namespace BankList.Controllers
{
    public class PincodeController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public PincodeController(IWebHostEnvironment env)
        {
            _env = env;
        }


        //// Step 1: Get pincode from lat/lng using Google API
        //private async Task<string> GetPincodeFromLatLng(double lat, double lng)
        //{
        //    string apiKey = "AIzaSyA9wOiyHkcc22LW7UV1iDTe7bxkPHyFGIY"; // Replace with your key
        //    string url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lng}&key={apiKey}";

        //    using (var client = new HttpClient())
        //    {
        //        var response = await client.GetStringAsync(url);
        //        var json = JsonDocument.Parse(response);

        //        foreach (var result in json.RootElement.GetProperty("results").EnumerateArray())
        //        {
        //            foreach (var comp in result.GetProperty("address_components").EnumerateArray())
        //            {
        //                var types = comp.GetProperty("types").EnumerateArray().Select(x => x.GetString()).ToList();
        //                if (types.Contains("postal_code"))
        //                {
        //                    return comp.GetProperty("long_name").GetString();
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}

        //// Step 2: Fetch banks by PINCODE (even if CSV has no PINCODE column)
        //[HttpGet("GetBankByPincode")]
        //public IActionResult GetBankByPincode(string pincode)
        //{
        //    if (string.IsNullOrWhiteSpace(pincode))
        //        return BadRequest(new { message = "Please provide a pincode." });

        //    var path = Path.Combine(_env.WebRootPath, "data", "bnk.csv");
        //    if (!System.IO.File.Exists(path))
        //        return NotFound(new { message = "CSV file not found." });

        //    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        //    {
        //        HasHeaderRecord = true,
        //        IgnoreBlankLines = true,
        //        TrimOptions = CsvHelper.Configuration.TrimOptions.Trim
        //    };

        //    List<BankDetail> results = new List<BankDetail>();

        //    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //    using (var reader = new StreamReader(stream))
        //    using (var csv = new CsvHelper.CsvReader(reader, config))
        //    {
        //        var records = csv.GetRecords<BankDetail>();

        //        // Extract last 6 digits from ADDRESS as virtual pincode
        //        results = records.Where(b =>
        //        {
        //            if (string.IsNullOrEmpty(b.ADDRESS)) return false;
        //            // Regex to find last 6 digit number in ADDRESS
        //            var match = Regex.Match(b.ADDRESS, @"(\d{6})\b");
        //            return match.Success && match.Value == pincode;
        //        }).ToList();
        //    }

        //    if (results.Count == 0)
        //        return NotFound(new { message = $"No banks found for pincode {pincode}" });

        //    return Ok(results);
        //}

        //// Step 3: Optional: Get bank by location (lat/lng)
        //[HttpGet("GetBankByLocation")]
        //public async Task<IActionResult> GetBankByLocation(double lat, double lng)
        //{
        //    string pincode = await GetPincodeFromLatLng(lat, lng);
        //    if (string.IsNullOrEmpty(pincode))
        //        return NotFound(new { message = "Could not determine pincode from location." });

        //    // Reuse the pincode search
        //    return GetBankByPincode(pincode);
        //}









        private readonly string apiKey = "YOUR_GOOGLE_API_KEY"; // apna key yaha daalo
        // GET: api/GoogleBank/Search?bankName=HDFC&pinCode=411030
        [HttpGet("Search")]
        public async Task<IActionResult> SearchBank(string bankName, string pinCode)
        {
            if (string.IsNullOrWhiteSpace(bankName) || string.IsNullOrWhiteSpace(pinCode))
                return BadRequest(new { message = "Please provide both bank name and pincode." });

            string query = $"{bankName} bank {pinCode}";
            string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={query}&key={apiKey}";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var json = JsonDocument.Parse(response);

                var results = json.RootElement.GetProperty("results");

                if (results.GetArrayLength() == 0)
                    return NotFound(new { message = $"No banks found for {bankName} in {pinCode}" });

                var banks = results.EnumerateArray().Select(place => new
                {
                    BANK_NAME = place.GetProperty("name").GetString(),
                    ADDRESS = place.GetProperty("formatted_address").GetString(),
                    PLACE_ID = place.TryGetProperty("place_id", out var placeId) ? placeId.GetString() : null,
                    LAT = place.GetProperty("geometry").GetProperty("location").GetProperty("lat").GetDouble(),
                    LNG = place.GetProperty("geometry").GetProperty("location").GetProperty("lng").GetDouble()
                }).ToList();

                return Ok(banks);
            }
        }



    }
}
