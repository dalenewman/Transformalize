using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.WebPages;
using Orchard;
using Orchard.DisplayManagement.Implementation;
using Orchard.Logging;
using Orchard.Templates.Compilation.Razor;
using Orchard.Templates.Services;

namespace Pipeline.Web.Orchard.Impl {
    public class RazorReportTemplateProcessor : TemplateProcessorImpl {

        private readonly IRazorCompiler _compiler;
        private readonly HttpContextBase _httpContextBase;
        private readonly IOrchardServices _orchardServices;
        public override string Type => "RazorReport";

        public RazorReportTemplateProcessor(
            IRazorCompiler compiler, 
            HttpContextBase httpContextBase,
            IOrchardServices orchardServices) {
            _orchardServices = orchardServices;
            _compiler = compiler;
            _httpContextBase = httpContextBase;
            Logger = NullLogger.Instance;
        }

        ILogger Logger { get; set; }

        public override void Verify(string template) {
            _compiler.CompileRazor(template, null, new Dictionary<string, object>());
        }

        public override string Process(string template, string name, DisplayContext context = null, dynamic model = null) {
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            var compiledTemplate = _compiler.CompileRazor(template, name, new Dictionary<string, object>());
            if (model != null) {
                model.OrchardUrl = (_httpContextBase.Request.Url?.PathAndQuery ?? string.Empty);
                model.OrchardUser = (_orchardServices.WorkContext.CurrentUser?.UserName ?? string.Empty);
                model.OrchardEmail = (_orchardServices.WorkContext.CurrentUser?.Email ?? string.Empty);
                model.OrchardTimeZone = (_orchardServices.WorkContext.CurrentSite?.SiteTimeZone ?? string.Empty);
            }
            var result = ActivateAndRenderTemplate(compiledTemplate, model);
            return result;
        }

        private string ActivateAndRenderTemplate(IRazorTemplateBase obj, object model) {
            var buffer = new StringBuilder(1024);
            using (var writer = new StringWriter(buffer)) {
                using (var htmlWriter = new HtmlTextWriter(writer)) {
                    var viewData = new ViewDataDictionary(model);
                    obj.ViewContext = new ViewContext(
                        new ControllerContext(
                            _httpContextBase.Request.RequestContext,
                            new StubController()),
                            new StubView(),
                            viewData,
                            new TempDataDictionary(),
                            htmlWriter
                    );

                    obj.ViewData = viewData;
                    obj.WebPageContext = new WebPageContext(_httpContextBase, obj as WebPageRenderingBase, model);
                    obj.WorkContext = new StubWorkContext();
                    obj.VirtualPath = "~/";

                    obj.Render(htmlWriter);
                }
            }
            return buffer.ToString();
        }

        private class StubController : Controller { }

        private class StubView : IView {
            public void Render(ViewContext viewContext, TextWriter writer) { }
        }

        private class StubWorkContext : WorkContext {

            public string CultureName { get; set; }
            public string CalendarName { get; set; }
            public TimeZoneInfo TimeZone { get; set; }

            public override T Resolve<T>() {
                throw new NotImplementedException();
            }

            public override object Resolve(Type serviceType) {
                throw new NotImplementedException();
            }

            public override bool TryResolve<T>(out T service) {
                throw new NotImplementedException();
            }

            public override bool TryResolve(Type serviceType, out object service) {
                throw new NotImplementedException();
            }

            public override T GetState<T>(string name) {
                if (name == "CurrentCulture") return (T)((object)CultureName);
                if (name == "CurrentCalendar") return (T)((object)CalendarName);
                if (name == "CurrentTimeZone") return (T)((object)TimeZone);
                throw new NotImplementedException($"Property '{name}' is not implemented.");
            }

            public override void SetState<T>(string name, T value) {
                throw new NotImplementedException();
            }
        }

        

    }


}