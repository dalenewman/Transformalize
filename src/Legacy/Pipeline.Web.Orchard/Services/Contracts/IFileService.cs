using System.Collections.Generic;
using Orchard;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Services.Contracts {
    public interface IFileService : IDependency {
        PipelineFilePart Upload(System.Web.HttpPostedFileBase input, string role, string tag, int number);
        PipelineFilePart Create(string name, string extension);
        PipelineFilePart Create(string fileName);
        IEnumerable<PipelineFilePart> List(string tag, int top = 15);
        PipelineFilePart Get(int id);
        void Delete(PipelineFilePart part);
    }
}



