using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Handlers {

    public class ScopeSearchSettingsPartHandler : ContentHandler {

        public Localizer T { get; set; }

        public ScopeSearchSettingsPartHandler(
            IRepository<PipelineSettingsPartRecord> repository
        ) {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            Filters.Add(new ActivatingFilter<PipelineSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T(Common.ModuleGroupName)));
        }

    }
}