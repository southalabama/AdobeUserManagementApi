using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class RemoveFromOrg
    {
        [JsonProperty("deleteAccount")]
        public bool DeleteAccount { get; set; }
    }
}