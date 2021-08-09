using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class Remove
    {
        [JsonProperty("user")]
        public string[] User { get; set; }
        [JsonProperty("productConfiguration")]
        public string[] ProductConfiguration { get; set; }
    }
}