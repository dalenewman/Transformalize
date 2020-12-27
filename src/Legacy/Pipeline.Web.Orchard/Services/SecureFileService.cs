using Orchard;
using Orchard.Core.Contents;
using Pipeline.Web.Orchard.Services.Contracts;

namespace Pipeline.Web.Orchard.Services {
    public class SecureFileService : ISecureFileService {

        private readonly IOrchardServices _orchardServices;
        private readonly IFileService _fileService;

        public SecureFileService(IOrchardServices orchardServices, IFileService fileService) {
            _orchardServices = orchardServices;
            _fileService = fileService;
        }

        public FileResponse Get(int id) {
            var response = new FileResponse();

            var part = _fileService.Get(id);

            if (part == null)
                return new FileResponse { Status = 404, Message = "The file does not exist." };

            if (string.IsNullOrEmpty(part.FullPath))
                return new FileResponse { Status = 404, Message = "The file path is empty." };

            if (!_orchardServices.Authorizer.Authorize(Permissions.ViewContent, part)) {
                return new FileResponse(401, "You do not have permissions to view this file.");
            }

            response.Part = part;

            if (response.Part.IsOnDisk()) {
                response.Status = 200;
                response.Message = "Ok";
            } else {
                response.Status = 404;
                response.Message = "The file does not exist anymore.";
            }

            return response;
        }
    }
}