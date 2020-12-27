using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace Pipeline.Web.Orchard.Models {
    public class PipelineSettingsPartRecord : ContentPartRecord {

        [StringLength(128)]
        public virtual string EditorTheme { get; set; }

        [StringLength(128)]
        public virtual string MapBoxToken { get; set; }

        public virtual double StartingLatitude { get; set; }

        public virtual double StartingLongitude { get; set; }

    }
}