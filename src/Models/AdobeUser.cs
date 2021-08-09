using Newtonsoft.Json;

// These classes were auto-generated in visual studio using Edit -> Paste Special -> Paste JSON As Classes
// The JSON used is below

#region example json
//      {
//          "id":"XXXXXXXX@AdobeID",
//          "email":"USER@EXAMPLE.COM",
//          "status":"active",
//          "groups":[
//              "_org_admin"
//              ],
//          "username":"USER@EXAMPLE.COM",
//          "adminRoles":["org"],
//          "domain":"EXAMPLE.COM",
//          "firstname":"Example",
//          "lastname":"User",
//          "country":"US",
//          "type":"adobeID"
//  }
#endregion

namespace AdobeUserManagementApi
{
    public class AdobeUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("groups")]
        public string[] Groups { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("adminRoles")]
        public string[] AdminRoles { get; set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}