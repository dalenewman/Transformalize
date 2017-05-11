using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services.Contracts;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Pipeline.Web.Orchard.Services {
    public class BatchCreateService : IBatchCreateService {

        private const string BatchCreateIndicator = "BatchCreate";
        private readonly IOrchardServices _orchardServices;
        private readonly IProcessService _processService;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public BatchCreateService(IOrchardServices services, IProcessService processService) {
            _orchardServices = services;
            _processService = processService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public IDictionary<string, string> Create(Process process, IDictionary<string, string> parameters) {

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var batchCreate = process.Actions.FirstOrDefault(a => a.Description.Equals(BatchCreateIndicator, StringComparison.OrdinalIgnoreCase));

            if (!(batchCreate?.Id > 0)) {
                return result;
            }

            var part = _orchardServices.ContentManager.Get(batchCreate.Id).As<PipelineConfigurationPart>();

            var creator = _processService.Resolve(part.EditorMode, part.EditorMode);

            creator.Load(part.Configuration, parameters);

            if (creator.Errors().Any()) {
                foreach (var error in creator.Errors()) {
                    _orchardServices.Notifier.Add(NotifyType.Error, T(error));
                    Logger.Error(error);
                }
            } else {
                try {
                    _orchardServices.WorkContext.Resolve<IRunTimeExecute>().Execute(creator);
                    var entity = creator.Entities.First();
                    var row = entity.Rows.First();
                    return entity.GetAllOutputFields().ToDictionary(k => k.Alias, v => row[v.Alias].ToString());
                } catch (Exception ex) {
                    Logger.Error(ex, ex.Message);
                }
            }
            return result;
        }
    }
}