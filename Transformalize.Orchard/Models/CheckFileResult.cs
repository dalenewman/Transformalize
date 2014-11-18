using System.IO;
using System.Web.Mvc;

namespace Transformalize.Orchard.Models {
    public class CheckFileResult {
        public ActionResult ActionResult { get; set; }
        public FilePart FilePart { get; set; }
        public FileInfo FileInfo { get; set; }

        public CheckFileResult(ActionResult actionResult, FilePart filePart = null, FileInfo fileInfo = null) {
            this.ActionResult = actionResult;
            this.FilePart = filePart;
            this.FileInfo = fileInfo;
        }

    }

   
}