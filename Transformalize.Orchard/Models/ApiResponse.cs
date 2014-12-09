using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Transformalize.Main;
using Transformalize.Orchard.Handlers;

namespace Transformalize.Orchard.Models {

    public class ApiResponse {

        private readonly ApiRequest _request;
        private readonly string _configuration;
        private readonly TransformalizeResponse _response = new TransformalizeResponse();
        private readonly string _metaData;

        public TransformalizeResponse TransformalizeResponse { get { return _response; } }

        public ApiResponse(ApiRequest request, string configuration)
        {
            _request = request;
            _configuration = configuration;
        }

        public ApiResponse(ApiRequest request, string configuration, string metaData) {
            _request = request;
            _configuration = configuration;
            _metaData = metaData;
        }

        public ApiResponse(ApiRequest request, string configuration, TransformalizeResponse response) {
            _response = response;
            _request = request;
            _configuration = configuration;
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
                    return JsonContentHandler.GetContent(_request, _configuration, _response, _metaData);
                default:
                    return XmlContentHandler.GetContent(_request, _configuration, _response, _metaData);
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