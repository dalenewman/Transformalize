using System;
using System.Collections.Generic;
using System.Web;
using Pipeline.Configuration;

namespace Pipeline.Web.Orchard {
    public static class Common {

        public static IDictionary<string, string> GetParameters(HttpRequestBase request) {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (request != null && request.QueryString != null) {
                foreach (string key in request.QueryString) {
                    parameters[key] = request.QueryString[key];
                }
            }
            return parameters;
        }

        public static void PageHelper(Process process, HttpRequestBase request) {
            if (request.QueryString["page"] == null) {
                return;
            }

            var page = 0;
            if (!int.TryParse(request.QueryString["page"], out page) || page <= 0) {
                return;
            }

            var size = 0;
            if (!int.TryParse((request.QueryString["size"] ?? "0"), out size)) {
                return;
            }

            foreach (var entity in process.Entities) {
                entity.Page = page;
                entity.PageSize = size > 0 ? size : entity.PageSize;
            }
        }
    }
}