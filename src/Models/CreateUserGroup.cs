using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class CreateUserGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("option")]
        public string Option { get; set; }
    }
}