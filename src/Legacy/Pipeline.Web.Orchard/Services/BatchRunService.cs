using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services.Contracts;
using Transformalize.Contracts;

namespace Pipeline.Web.Orchard.Services {
    public class BatchRunService : IBatchRunService {

        private readonly IOrchardServices _orchardServices;
        private readonly IProcessService _processService;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public BatchRunService(IOrchardServices services, IProcessService processService) {
            _orchardServices = services;
            _processService = processService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public bool Run(Transformalize.Configuration.Action action, IDictionary<string, string> parameters) {

            var part = _orchardServices.ContentManager.Get(action.Id).As<PipelineConfigurationPart>();
            if (part == null) {
                if (action.Url == string.Empty) {
                    const string msg = "The action doesn't refer to a valid 'id' or redirect 'url'.";
                    Logger.Error(msg);
                    _orchardServices.Notifier.Error(T(msg));
                    return false;
                }
                return true;
            }

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                _orchardServices.Notifier.Warning(T("You do not have permission to run this bulk action."));
                return false;
            }

            var actionProcess = _processService.Resolve(part);

            actionProcess.Load(part.Configuration, parameters);

            if (actionProcess.Errors().Any()) {
                foreach (var error in actionProcess.Errors()) {
                    _orchardServices.Notifier.Add(NotifyType.Error, T(error));
                    Logger.Error(error);
                }
                return false;
            }

            try {
                _orchardServices.WorkContext.Resolve<IRunTimeExecute>().Execute(actionProcess);
                return true;
            } catch (Exception ex) {
                Logger.Error(ex, ex.Message);
                _orchardServices.Notifier.Error(T(ex.Message));
                return false;
            }
        }
    }
}