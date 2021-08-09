using Newtonsoft.Json;
// These classes were auto-generated in visual studio using Edit -> Paste Special -> Paste JSON As Classes
// The JSON used is below

#region JSON Example
//{
//    "usergroup": "DevOps",
//  "do": [
//     {
//        "add": {
//            "user": [
//              "user1@myCompany.com"
//          ],
//        "productConfiguration": [
//          "Profile1_Name"
//          ],
//      }
//    },
//     {
//        "remove": {
//            "user": [
//              "user2@myCompany.com"
//          ],
//        "productConfiguration": [
//          "Profile2_Name"
//          ],        
//       }
//    }
//  ]
//}
#endregion

namespace AdobeUserManagementApi
{
    public class GroupAction
    {
        [JsonProperty("usergroup")]
        public string UserGroup { get; set; }
        // This field has a leading underscore because do is a reserved keyword in C#
        // But when serializing, we should correct it because adobe API expects "do" not "_do"
        [JsonProperty("do")]
        public Do[] _do { get; set; }
    }
}