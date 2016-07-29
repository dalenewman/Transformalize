using System;
using System.Linq;
using Cfg.Net.Shorthand;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Pipeline.Web.Orchard.Impl;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Handlers {

    public class ScopeSearchSettingsPartHandler : ContentHandler {

        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public ScopeSearchSettingsPartHandler(
            IRepository<PipelineSettingsPartRecord> repository,
            INotifier notifier
        ) {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            _notifier = notifier;
            Filters.Add(new ActivatingFilter<PipelineSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T(Common.ModuleGroupName)));
        }

        protected override void Updated(UpdateContentContext context) {

            var part = context.ContentItem.As<PipelineSettingsPart>();
            if (part == null)
                return;

            try {
                var cfg = new ShorthandRoot(part.Shorthand, new CfgNetNotifier(_notifier));
                if (!cfg.Errors().Any()) {
                    return;
                }
                _notifier.Add(NotifyType.Error, T("Shorthand is invalid."));
                foreach (var error in cfg.Errors()) {
                    _notifier.Add(NotifyType.Error, T(error));
                }
            } catch (Exception ex) {
                _notifier.Add(NotifyType.Error, T(ex.Message));
                Logger.Error(ex, ex.Message);
            }
        }
    }
}