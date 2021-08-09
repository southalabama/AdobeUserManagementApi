using Newtonsoft.Json;
// These classes were auto-generated in visual studio using Edit -> Paste Special -> Paste JSON As Classes
// The JSON used is below, info partially grabbed from the link below
// https://adobe-apiplatform.github.io/umapi-documentation/en/api/ActionsRef.html

#region example json 
//{
//    "completed":1,
//   "notCompleted":0,
//   "completedInTestMode":0,
//   "result":"success",
//   "errors": [
//       {
//           "index": 1,
//         "step": 0,
//         "requestID": "Two2_123456",
//         "message": "User Id does not exist: test@test_fake.us",
//         "user": "test@test_fake.us",
//         "errorCode": "error.user.nonexistent"
//       }
//     ],
//   "warnings": [
//       {
//           "warningCode": "warning.command.deprecated",
//         "requestID": "Ten10_123456",
//         "index": 9,
//         "step": 0,
//         "message": "'product' command is deprecated. Please use productConfiguration.",
//         "user": "user10@example.com"
//       }
//     ]
//}
#endregion example json

namespace AdobeUserManagementApi
{
    public class AdobeApiResponse
    {
        //  The number of user commands that were successful.
        [JsonProperty("completed")]
        public int Completed { get; set; }
        // The number of user commands that were unsuccessful. When non-zero the errors field lists the specific actions that failed, with error information.
        [JsonProperty("notCompleted")]
        public int NotCompleted { get; set; }
        //  The number of users that were completed in testOnly mode.
        [JsonProperty("completedInTestMode")]
        public int CompletedInTestMode { get; set; }
        
        // Possible values:
        // success: All the actions were completed.completed field will equal the total of commands processed.
        // partial: Some of the actions failed.completed and notCompleted fields with identify the number of commands that succeeded and failed.
        // error: All the requested actions failed.completed will be 0 and notCompleted will show the number of requests that failed.
        [JsonProperty("result")]
        public string Result { get; set; }

        // An array of errors. Each error entry is an object with the attributes below. This section is ommitted if no errors were generated.
        [JsonProperty("errors")]
        public Error[] Errors { get; set; }

        // An array of warnings. Each warning entry is an object with the attributes below. This section is ommitted if no warnings were generated.
        [JsonProperty("warnings")]
        public Warning[] Warnings { get; set; }
    }
}
