using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Core.Title.Models;
using Orchard.Tags.Models;
using Pipeline.Web.Orchard.Models;
using Pipeline.Web.Orchard.Services.Contracts;

namespace Pipeline.Web.Orchard.Services {

    public class CfgService : ICfgService {
        private readonly IOrchardServices _orchardServices;

        public CfgService(IOrchardServices orchardServices) {
            _orchardServices = orchardServices;
        }

        public IEnumerable<PipelineConfigurationPart> List(string tag) {

            if (string.IsNullOrEmpty(tag) || tag == Common.AllTag) {
                return _orchardServices.ContentManager
                    .Query<PipelineConfigurationPart, PipelineConfigurationPartRecord>(VersionOptions.Published)
                    .Join<TitlePartRecord>()
                    .OrderBy(br => br.Title)
                    .List()
                    .Where(c => _orchardServices.Authorizer.Authorize(Permissions.ViewContent, c));
            }

            return _orchardServices.ContentManager.Query<PipelineConfigurationPart, PipelineConfigurationPartRecord>(VersionOptions.Published)
                .Join<TagsPartRecord>()
                .Where(tpr => tpr.Tags.Any(t => t.TagRecord.TagName.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                .Join<TitlePartRecord>()
                .OrderBy(br => br.Title)
                .List()
                .Where(c => _orchardServices.Authorizer.Authorize(Permissions.ViewContent, c));

        }
    }
}