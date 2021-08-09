using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class AddAdobeID 
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("firstname")]
        public string Firstname { get; set; }
        [JsonProperty("lastname")]
        public string Lastname { get; set; }
        [JsonProperty("option")]
        public string Option { get; set; }
    }
}