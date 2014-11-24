using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Transformalize.Main;
using Transformalize.Orchard.Handlers;

namespace Transformalize.Orchard.Models {

    public class ApiResponse {

        private readonly ApiRequest _request;
        private readonly TransformalizeResponse _response = new TransformalizeResponse();
        private readonly string _metaData;

        public ApiResponse(ApiRequest request) {
            _request = request;
        }

        public ApiResponse(ApiRequest request, string metaData) {
            _request = request;
            _metaData = metaData;
        }

        public ApiResponse(ApiRequest request, TransformalizeResponse response) {
            _response = response;
            _request = request;
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
                    return JsonContentHandler.GetContent(_request, _response, _metaData);
                default:
                    return XmlContentHandler.GetContent(_request, _response, _metaData);
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