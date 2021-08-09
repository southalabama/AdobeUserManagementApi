using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class AdobeGroup
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("memberCount")]
        public int MemberCount { get; set; }
        [JsonProperty("productName")]
        public string ProductName { get; set; }
        [JsonProperty("productProfileName")]
        public string ProductProfileName { get; set; }
        [JsonProperty("licenseQuota")]
        public string LicenseQuota { get; set; }
    }
}