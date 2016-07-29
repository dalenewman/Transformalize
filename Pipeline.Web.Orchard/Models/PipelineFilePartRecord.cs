using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace Pipeline.Web.Orchard.Models {
    public class PipelineFilePartRecord : ContentPartRecord {

        [StringLength(512)]
        public virtual string FullPath { get; set; }

        [StringLength(3)]
        public virtual string Direction { get; set; }
       
    }
}