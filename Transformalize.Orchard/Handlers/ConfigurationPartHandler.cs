using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Transformalize.Configuration;
using Transformalize.Orchard.Models;
using Transformalize.Orchard.Services;

namespace Transformalize.Orchard.Handlers {
    public class ConfigurationPartHandler : ContentHandler {
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public ConfigurationPartHandler(
            IRepository<ConfigurationPartRecord> repository,
            INotifier notifier
            ) {
            _notifier = notifier;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Filters.Add(StorageFilter.For(repository));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<ConfigurationPart>();

            if (part == null)
                return;

            base.GetItemMetadata(context);
            context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                {"Area", "Transformalize.Orchard"},
                {"Controller", "Transformalize"},
                {"Action", "Configuration"},
                {"id", context.ContentItem.Id}
            };
        }

        protected override void Updated(UpdateContentContext context) {
            var part = context.ContentItem.As<ConfigurationPart>();
            if (part == null)
                return;
            try {
                //test if configuration works
                var root = new TflRoot(
                    part.Configuration,
                    null,
                    new CfgNetNotifier(_notifier)
                );
                CheckAddress(part.StartAddress);
                CheckAddress(part.EndAddress);
                Logger.Information("Loaded {0} with {1} warnings, and {2} errors.", part.Title(), root.Warnings().Length, root.Errors().Length);
            } catch (Exception ex) {
                _notifier.Add(NotifyType.Warning, T(ex.Message));
                Logger.Warning(ex.Message);
            }
        }

        private void CheckAddress(string ipAddress) {
            if (string.IsNullOrEmpty(ipAddress)) {
                return;
            }
            IPAddress start;
            if (IPAddress.TryParse(ipAddress, out start)) {
                return;
            }
            _notifier.Add(NotifyType.Warning, T("{0} is an invalid address.", ipAddress));
        }

    }

}
