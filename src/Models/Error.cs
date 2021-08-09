using Newtonsoft.Json;

namespace AdobeUserManagementApi
{
    public class Error
    {
        // The 0-based index of the command entry in the commands structure.
        [JsonProperty("index")]
        public int Index { get; set; }
        // The 0-based index of the action step within that command entry.
        [JsonProperty("step")]
        public int Step { get; set; }
        // A developer-defined ID passed into the request which you can use to match this response to a specific request.
        [JsonProperty("requestID")]
        public string RequestID { get; set; }
        // A description of the error.
        [JsonProperty("message")]
        public string Message { get; set; }
        // The user defined in the root of the command entry.
        [JsonProperty("user")]
        public string User { get; set; }
        // The error type. See https://adobe-apiplatform.github.io/umapi-documentation/en/api/ErrorRef.html for a full list.
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }
    }
}