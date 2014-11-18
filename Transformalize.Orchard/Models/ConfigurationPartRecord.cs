using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Transformalize.Orchard.Models {

    public class ConfigurationPartRecord : ContentPartRecord {
        [StringLengthMax]
        public virtual string Configuration { get; set; }

        [StringLength(128)]
        public virtual string Modes { get; set; }

        public virtual bool TryCatch { get; set; }

        public virtual bool DisplayLog { get; set; }

        [StringLength(32)]
        public virtual string LogLevel { get; set; }

        [StringLength(10)]
        public virtual string OutputFileExtension { get; set; }

        [StringLength(64)]
        public virtual string StartAddress { get; set; }

        [StringLength(64)]
        public virtual string EndAddress { get; set; }

    }
}