using System.Collections.Specialized;
using System.Diagnostics;

namespace Transformalize.Orchard.Models {

    public class ApiRequest {
        public ApiRequestType RequestType { get; set; }
        public string Configuration { get; set; }
        public Stopwatch Stopwatch { get; set; }
        public NameValueCollection Query { get; set; }

        public ApiRequest(ApiRequestType requestType) {
            RequestType = requestType;
            Query = new NameValueCollection();
            Configuration = string.Empty;
            Stopwatch = new Stopwatch();
        }
    }
}