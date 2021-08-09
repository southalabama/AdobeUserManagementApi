using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class Update
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
    }
}