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
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Handlers {

    public class PipelineFilePartHandler : ContentHandler {

        public Localizer T { get; set; }

        public PipelineFilePartHandler(
            IRepository<PipelineFilePartRecord> repository
        ) {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Filters.Add(StorageFilter.For(repository));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<PipelineFilePart>();

            if (part == null)
                return;

            base.GetItemMetadata(context);
            context.Metadata.DisplayRouteValues = new RouteValueDictionary {
                {"Area", Common.ModuleName},
                {"Controller", "File"},
                {"Action", "File/View"},
                {"id", context.ContentItem.Id}
            };
        }

    }

}
