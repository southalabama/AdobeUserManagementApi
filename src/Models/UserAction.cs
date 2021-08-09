using Newtonsoft.Json;
// These classes were auto-generated in visual studio using Edit -> Paste Special -> Paste JSON As Classes
// The JSON used is below

#region JSON Example
//[{
//  "user" : "jdoe@claimed-domain1.com",
//  "requestID": "action_1",
//  "do" : [{
//    "createEnterpriseID": {
//      "email": "jdoe@dclaimed-domain1.com",
//      "country": "US",
//      "firstname": "John",
//      "lastname": "Doe",
//      "option": "ignoreIfAlreadyExists"
//    }
//  }]
//}]
#endregion

namespace AdobeUserManagementApi
{

    public class UserAction
    {
        [JsonProperty("user")]
        public string User { get; set; }
        [JsonProperty("requestID")]
        public string RequestID { get; set; }
        [JsonProperty("do")]
        public Do[] _do { get; set; }
    }
}