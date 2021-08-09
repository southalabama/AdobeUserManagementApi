using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class GroupResponse
    {
        [JsonProperty("lastPage")]
        public bool LastPage { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("groups")]
        public AdobeGroup[] Groups { get; set; }
    }
}