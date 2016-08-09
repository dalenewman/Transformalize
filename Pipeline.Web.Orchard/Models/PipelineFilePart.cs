using System;
using System.Collections.Generic;
using System.IO;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Environment.Extensions;
using Orchard.Tags.Models;

namespace Pipeline.Web.Orchard.Models {

    [OrchardFeature("Pipeline.Files")]
    public class PipelineFilePart : ContentPart<PipelineFilePartRecord> {

        public string Title() {
            return this.As<TitlePart>().Title;
        }

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

        public string Extension() {
            return Path.GetExtension(FileName());
        }

        public string MimeType() {
            return Common.GetMimeType(Extension());
        }

        public IEnumerable<string> Tags() {
            return this.As<TagsPart>().CurrentTags;
        }

        public bool IsOnDisk() {
            return FullPath != string.Empty && File.Exists(FullPath);
        }

    }
}