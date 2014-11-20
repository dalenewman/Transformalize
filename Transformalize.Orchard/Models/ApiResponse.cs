using System;
using System.Web.Mvc;
using Transformalize.Main;
using Transformalize.Orchard.Handlers;

namespace Transformalize.Orchard.Models {

    [Serializable]
    public class ApiResponse {

        private readonly ApiRequest _request;
        private readonly Process[] _processes = new Process[0];
        private readonly string _metaData;

        public int Status { get; set; }
        public string Message { get; set; }

        public ApiResponse(ApiRequest request) {
            _request = request;
            Status = 200;
            Message = "OK";
        }

        public ApiResponse(ApiRequest request, string metaData) {
            _request = request;
            _metaData = metaData;
            Status = 200;
            Message = "OK";
        }

        public ApiResponse(ApiRequest request, Process[] processes) {
            _processes = processes;
            _request = request;
            Status = 200;
            Message = "OK";
        }

        public ContentResult ContentResult(string format, string flavor = null) {
            return new ContentResult() {
                Content = Content(format, flavor),
                ContentType = MimeType(format)
            };
        }

        private string Content(string format, string flavor) {
            _request.Flavor = flavor.ToLower();
            switch (format.ToLower()) {
                case "json":
                    return JsonContentHandler.GetContent(_request, _processes, _metaData);
                default:
                    return XmlContentHandler.GetContent(_request, _processes, _metaData);
            }
        }

        private static string MimeType(string format) {
            switch (format.ToLower()) {
                case "json":
                    return "application/json";
                default:
                    return "text/xml";
            }
        }

    }
}