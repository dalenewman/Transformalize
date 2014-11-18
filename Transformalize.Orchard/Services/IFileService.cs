using System.Collections.Generic;
using Orchard;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {

    public interface IFileService : IDependency {
        FilePart Upload(System.Web.HttpPostedFileBase input);
        FilePart Create(string name, string extension);
        IEnumerable<FilePart> GetFiles();
        FilePart Get(int id);
    }
}



