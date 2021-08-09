using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class Do
    {
        [JsonProperty("add")]
        public Add Add { get; set; }
        [JsonProperty("remove")]
        public Remove Remove { get; set; }
        [JsonProperty("createEnterpriseID")]
        public CreateEnterpriseID CreateEnterpriseID { get; set; }
        [JsonProperty("createFederatedID")]
        public CreateFederatedID CreateFederatedID { get; set; }
        [JsonProperty("addAdobeID")]
        public AddAdobeID AddAdobeID { get; set; }
        [JsonProperty("removeFromOrg")]
        public RemoveFromOrg RemoveFromOrg { get; set; }
        [JsonProperty("update")]
        public Update Update { get; set; }
        [JsonProperty("createUserGroup")] 
        public CreateUserGroup CreateUserGroup { get; set; }
        [JsonProperty("updateUserGroup")] 
        public UpdateUserGroup UpdateUserGroup { get; set; }
        [JsonProperty("deleteUserGroup")] 
        public UpdateUserGroup DeleteUserGroup { get; set; }
    }
}