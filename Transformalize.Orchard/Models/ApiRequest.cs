using System.Diagnostics;

namespace Transformalize.Orchard.Models {

    public class ApiRequest {
        public ApiRequestType RequestType { get; set; }
        public Stopwatch Stopwatch { get; set; }
        public string Flavor { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }

        public ApiRequest(ApiRequestType requestType) {
            Stopwatch = new Stopwatch();
            RequestType = requestType;
            Flavor = string.Empty;
            Status = 200;
            Message = "OK";
        }
    }
}