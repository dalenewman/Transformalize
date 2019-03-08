using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Pipeline.Web.Orchard.Controllers {

    public class BaseController : Controller {

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public BaseController() {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

        }

        public bool IsMissingRequiredParameters(List<Transformalize.Configuration.Parameter> parameters, INotifier notifier) {

            var hasRequiredParameters = true;
            foreach (var parameter in parameters.Where(p => p.Required)) {

                var value = Request.QueryString[parameter.Name];
                if (value != null && value != "*") {
                    continue;
                }

                if (parameter.Sticky && parameter.Value != "*") {
                    continue;
                }

                notifier.Add(NotifyType.Warning, T("{0} is required. To continue, please choose a {0}.", parameter.Label));
                if (hasRequiredParameters) {
                    hasRequiredParameters = false;
                }
            }

            return !hasRequiredParameters;
        }

        public T GetStickyUrlParameter<T>(int partId, string name, Func<T> defaultValue) where T : IConvertible {

            var key = partId + name;

            if (Request.QueryString[name] != null) {
                try {
                    var tc = TypeDescriptor.GetConverter(typeof(T));
                    var queryValue = (T)tc.ConvertFromString(Request.QueryString[name]);
                    if (queryValue != null) {
                        if (!queryValue.Equals(Session[key])) {
                            Session[key] = queryValue;
                        }

                        return queryValue;
                    }
                } catch (Exception exception) {
                    Logger.Error(exception.Message);
                }
            }

            if (Request.Form[name] != null) {
                try {
                    var tc = TypeDescriptor.GetConverter(typeof(T));
                    var formValue = (T)tc.ConvertFromString(Request.Form[name]);
                    if (formValue != null) {
                        if (!formValue.Equals(Session[key])) {
                            Session[key] = formValue;
                        }
                        
                        return formValue;
                    }
                } catch (Exception exception) {
                    Logger.Error(exception.Message);
                }
            }

            if (Session[key] != null) {
                return (T)Session[key];
            }

            var value = defaultValue();
            Session[key] = value;
            return value;

        }

        public void SetStickyParameters(int id, List<Transformalize.Configuration.Parameter> parameters) {
            foreach (var parameter in parameters.Where(p => p.Sticky)) {
                var key = id + parameter.Name;
                if (Request.QueryString[parameter.Name] == null) {
                    if (Session[key] != null) {
                        parameter.Value = Session[key].ToString();
                    }
                } else {  // A parameter is set
                    var value = Request.QueryString[parameter.Name];
                    if (Session[key] == null) {
                        Session[key] = value;  // for the next time
                        parameter.Value = value; // for now
                    } else {
                        if (Session[key].ToString() != value) {
                            Session[key] = value; // for the next time
                            parameter.Value = value; // for now
                        }
                    }
                }
            }
        }

        public void GetStickyParameters(int id, IDictionary<string, string> parameters) {
            var prefix = id.ToString();
            foreach (string key in Session.Keys) {
                if (key.StartsWith(prefix)) {
                    var name = key.Substring(prefix.Length);
                    if (!parameters.ContainsKey(name) && Session[key] != null) {
                        parameters[name] = Session[key].ToString();
                    }
                }
            }
        }
    }
}