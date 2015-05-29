using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace Transformalize.Orchard.Controllers {
    public class TflController : Controller {
        protected const string DefaultFormat = "xml";
        protected const string DefaultFlavor = "attributes";
        protected const string DefaultMode = "default";

        protected static Dictionary<string, string> GetQuery() {
            var request = System.Web.HttpContext.Current.Request;
            var collection = new NameValueCollection { request.Form, request.QueryString };
            var result = new Dictionary<string, string>(collection.Count, StringComparer.Ordinal);
            foreach (var key in collection.AllKeys) {
                result[key] = collection[key];
            }
            if (!result.ContainsKey("flavor")) {
                result["flavor"] = DefaultFlavor;
            }
            if (!result.ContainsKey("format")) {
                result["format"] = DefaultFormat;
            }
            if (!result.ContainsKey("mode"))
            {
                result["mode"] = DefaultMode;
            }
            return result;
        }

    }
}