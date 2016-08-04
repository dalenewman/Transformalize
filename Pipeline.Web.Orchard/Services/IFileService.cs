using System.Collections.Generic;
using Orchard;
using Pipeline.Web.Orchard.Models;

namespace Pipeline.Web.Orchard.Services {
    public interface IFileService : IDependency {
        PipelineFilePart Upload(System.Web.HttpPostedFileBase input, string role);
        PipelineFilePart Create(string name, string extension);
        PipelineFilePart Create(string fileName);
        IEnumerable<PipelineFilePart> List();
        PipelineFilePart Get(int id);
        void Delete(PipelineFilePart part);
    }

}



