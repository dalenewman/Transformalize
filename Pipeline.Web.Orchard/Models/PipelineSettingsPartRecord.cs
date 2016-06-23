using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Pipeline.Web.Orchard.Models {
    public class PipelineSettingsPartRecord : ContentPartRecord {

        [StringLength(128)]
        public virtual string EditorTheme { get; set; }

        [StringLengthMax]
        public virtual string Shorthand { get; set; }

    }
}