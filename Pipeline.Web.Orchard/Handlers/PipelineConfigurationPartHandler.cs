#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
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
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services;

namespace Pipeline.Web.Orchard.Handlers {

    public class PipelineConfigurationPartHandler : ContentHandler {
        private readonly INotifier _notifier;
        private readonly IProcessService _processService;

        public Localizer T { get; set; }

        public PipelineConfigurationPartHandler(
            IRepository<PipelineConfigurationPartRecord> repository,
            IProcessService processService,
            INotifier notifier
        ) {
            _notifier = notifier;
            _processService = processService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Filters.Add(StorageFilter.For(repository));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<PipelineConfigurationPart>();

            if (part == null)
                return;

            base.GetItemMetadata(context);
            if (part.ReportMode()) {
                context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                    {"Area", Common.ModuleName},
                    {"Controller", "Cfg"},
                    {"Action", "Report"},
                    {"id", context.ContentItem.Id}
                };
            } else {
                if (part.FormMode()) {
                    context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                        {"Area", Common.ModuleName},
                        {"Controller", "Cfg"},
                        {"Action", "Form"},
                        {"id", context.ContentItem.Id}
                    };
                } else {
                    context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                        {"Area", Common.ModuleName},
                        {"Controller", "Api"},
                        {"Action", "Api/Cfg"},
                        {"id", context.ContentItem.Id}
                    };
                }
            }
        }

        protected override void Updated(UpdateContentContext context) {

            var part = context.ContentItem.As<PipelineConfigurationPart>();

            if (part == null)
                return;
            try {
                var root = _processService.Resolve(part);
                root.Load(part.Configuration);

                if (root.Errors().Any()) {
                    foreach (var error in root.Errors()) {
                        _notifier.Add(NotifyType.Error, T(error));
                    }
                }

                if (root.Warnings().Any()) {
                    foreach (var warning in root.Warnings()) {
                        _notifier.Add(NotifyType.Warning, T(warning));
                    }
                }

                CheckAddress(part.StartAddress);
                CheckAddress(part.EndAddress);

            } catch (Exception ex) {

                _notifier.Add(NotifyType.Error, T(ex.Message));
                Logger.Error(ex.Message);
            }
        }

        private void CheckAddress(string ipAddress) {
            if (string.IsNullOrEmpty(ipAddress)) {
                return;
            }

            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip)) {
                return;
            }
            _notifier.Add(NotifyType.Warning, T("{0} is an invalid address.", ipAddress));
        }

    }

}
