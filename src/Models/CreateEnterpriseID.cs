using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class CreateEnterpriseID
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("option")]
        public string Option { get; set; }
    }
}