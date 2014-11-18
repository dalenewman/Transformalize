using System;
using System.Linq;
using System.Net;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Transformalize.Configuration;
using Transformalize.Main;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Handlers {
    public class ConfigurationPartHandler : ContentHandler {
        private readonly INotifier _notifier;

        public Localizer T { get; set; }
        public ILogger Log { get; set; }

        public ConfigurationPartHandler(
            IRepository<ConfigurationPartRecord> repository,
            INotifier notifier
            ) {
            _notifier = notifier;
            T = NullLocalizer.Instance;
            Log = NullLogger.Instance;
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
                foreach (var process in new ConfigurationFactory(part.Configuration).Create().Cast<ProcessConfigurationElement>().Select(element => ProcessFactory.Create(element)).SelectMany(processes => processes)) {
                    Log.Information("Successfully loaded {0}.", process.Name);
                }
                CheckAddress(part.StartAddress);
                CheckAddress(part.EndAddress);
            } catch (Exception ex) {
                _notifier.Add(NotifyType.Warning, T(ex.Message));
                Log.Warning(ex.Message);
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
