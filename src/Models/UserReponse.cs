using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class UserResponse
    {
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("user")]
        public AdobeUser User { get; set; }
    }
}