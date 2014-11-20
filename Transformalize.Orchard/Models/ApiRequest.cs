using System.Diagnostics;

namespace Transformalize.Orchard.Models {

    public class ApiRequest {
        public ApiRequestType RequestType { get; set; }
        public string Configuration { get; set; }
        public Stopwatch Stopwatch { get; set; }
        public string Flavor { get; set; }

        public ApiRequest(ApiRequestType requestType) {
            RequestType = requestType;
            Configuration = string.Empty;
            Stopwatch = new Stopwatch();
            Flavor = string.Empty;
        }
    }
}