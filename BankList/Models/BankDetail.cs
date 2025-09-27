using System.Text.Json.Serialization;

namespace BankList.Models
{
    public class BankDetail
    {
        [JsonPropertyName("IFSC_CODE")]
        public string IFSC_CODE { get; set; }

        [JsonPropertyName("MICR_CODE")]
        public string MICR_CODE { get; set; }

        [JsonPropertyName("BRANCH_NAME")]
        public string BRANCH_NAME { get; set; }

        [JsonPropertyName("ADDRESS")]
        public string ADDRESS { get; set; }

        [JsonPropertyName("CONTACT")]
        public string CONTACT { get; set; }

        [JsonPropertyName("CITY")]
        public string CITY { get; set; }

        [JsonPropertyName("DISTRICT")]
        public string DISTRICT { get; set; }

        [JsonPropertyName("STATE")]
        public string STATE { get; set; }

        [JsonPropertyName("BANK_NAME")]
        public string BANK_NAME { get; set; }

        [JsonPropertyName("STATUS")]
        public string STATUS { get; set; }


    



}
}
