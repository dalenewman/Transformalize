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
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Controllers {

    public class ListController : Controller {

        private readonly IOrchardServices _orchardServices;
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ListController(
            IOrchardServices services
            ) {
            _orchardServices = services;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        [Themed]
        public ActionResult List() {

            if (!User.Identity.IsAuthenticated)
                System.Web.Security.FormsAuthentication.RedirectToLoginPage(Request.RawUrl);

            var configurations = _orchardServices.ContentManager
                .Query<PipelineConfigurationPart, PipelineConfigurationPartRecord>(VersionOptions.Latest)
                .Join<TitlePartRecord>()
                .OrderBy(br => br.Title)
                .List()
                .Where(c => _orchardServices.Authorizer.Authorize(global::Orchard.Core.Contents.Permissions.ViewContent, c))
                .ToArray();

            return View(configurations);
        }
    }
}