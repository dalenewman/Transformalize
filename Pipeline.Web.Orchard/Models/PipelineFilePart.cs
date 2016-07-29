using System;
using System.IO;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;

namespace Pipeline.Web.Orchard.Models {
    public class PipelineFilePart : ContentPart<PipelineFilePartRecord> {

        public string FullPath {
            get { return Record.FullPath; }
            set { Record.FullPath = value; }
        }

        public string Direction {
            get { return Record.Direction; }
            set { Record.Direction = value; }
        }

        public string FileName() {
            return string.IsNullOrEmpty(Record.FullPath) ? string.Empty : Path.GetFileName(Record.FullPath);
        }

        public bool IsValid() {
            return !string.IsNullOrEmpty(FullPath);
        }

        public DateTime CreatedUtc() {
            return this.As<CommonPart>().CreatedUtc ?? DateTime.UtcNow;
        }
    }
}