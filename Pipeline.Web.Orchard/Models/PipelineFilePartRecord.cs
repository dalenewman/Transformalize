using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace Pipeline.Web.Orchard.Models {

    [OrchardFeature("Pipeline.Files")]
    public class PipelineFilePartRecord : ContentPartRecord {

        [StringLength(512)]
        public virtual string FullPath { get; set; }

        [StringLength(3)]
        public virtual string Direction { get; set; }
       
    }
}