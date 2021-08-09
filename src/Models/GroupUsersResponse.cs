using Newtonsoft.Json;

// These classes were auto-generated in visual studio using Edit -> Paste Special -> Paste JSON As Classes
// The JSON used is below

#region JSON example
//{
//  "lastPage":true,
//  "result":"success",
//  "users":[
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
//  }],
//  "groupName":"Acrobat Pro DC Allowed"
//  }
#endregion

namespace AdobeUserManagementApi
{
    public class GroupUsersResponse
    {
        [JsonProperty("lastPage")]
        public bool LastPage { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("users")]
        public AdobeUser[] Users { get; set; }
        [JsonProperty("groupName")]
        public string GroupName { get; set; }
    }
}
