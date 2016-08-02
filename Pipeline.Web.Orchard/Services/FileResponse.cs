using System.IO;
using System.Web.Mvc;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Services {
    public class FileResponse {

        public PipelineFilePart Part { get; set; }
        public FileInfo FileInfo { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }

        public FileResponse(int status, string message) {
            Status = status;
            Message = message;
        }

        public FileResponse() {
            Part = null;
            Status = 404;
            Message = "File Not Found";
        }

        public ActionResult ToActionResult() {
            return new HttpStatusCodeResult(Status, Message);
        }
    }
}