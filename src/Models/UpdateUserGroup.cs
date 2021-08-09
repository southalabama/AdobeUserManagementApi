using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class UpdateUserGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}