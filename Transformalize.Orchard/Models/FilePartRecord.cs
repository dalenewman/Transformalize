using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement.Records;

namespace Transformalize.Orchard.Models {
    public class FilePartRecord : ContentPartRecord {

        [StringLength(512)]
        public virtual string FullPath { get; set; }

        [StringLength(3)]
        public virtual string Direction { get; set; }
       
    }
}