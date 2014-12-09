using System;
using System.Collections.Generic;

namespace Transformalize.Orchard.Models {
    public class FilesResponse {
        public IEnumerable<FilePart> Files { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }
}