using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Services.Contracts;
using Transformalize.Configuration;

namespace Pipeline.Web.Orchard.Services {
    public class BatchRedirectService : IBatchRedirectService {

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        private static readonly Regex _placeHolderMatcher = new Regex("(?<={)[^}]+(?=})", RegexOptions.Compiled);
        private readonly IOrchardServices _orchardServices;

        public BatchRedirectService(IOrchardServices orchardServices) {
            _orchardServices = orchardServices;
        }

        public ActionResult Redirect(Process process, IDictionary<string, string> parameters) {

            var redirect = process.Actions.FirstOrDefault(a => a.Description.Equals("BatchRedirect", StringComparison.OrdinalIgnoreCase));
            if (redirect == null) {
                return null;
            }

            var matches = _placeHolderMatcher.Matches(redirect.Url);
            if (matches.Count > 0) {

                var values = new List<string>();
                foreach (Match match in matches) {
                    if (!values.Contains(match.Value)) {
                        values.Add(match.Value);
                    }
                }

                var names = new List<string>();
                foreach (var value in values) {
                    var left = value.Split(':')[0];
                    if (left.ToCharArray().All(c => c >= '0' && c <= '9'))
                        continue;
                    if (!names.Contains(left)) {
                        names.Add(left);
                    }
                }

                var count = 0;
                var args = new List<object>();
                foreach (var name in names) {
                    if (parameters.ContainsKey(name)) {
                        args.Add(parameters[name]);
                        redirect.Url = redirect.Url.Replace("{" + name, "{" + count);
                        count++;
                    } else {
                        _orchardServices.Notifier.Error(T("Can not find parameter {0} for BatchRedirect url.", name));
                        return null;
                    }
                }
                redirect.Url = string.Format(redirect.Url, args.ToArray());
            }
            var url = new Flurl.Url(VirtualPathUtility.ToAbsolute(redirect.Url));
            foreach (var p in parameters) {
                url.QueryParams.Add(p.Key, p.Value);
            }
            return new RedirectResult(url.ToString());
        }
    }
}