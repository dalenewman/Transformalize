using System.Collections.Generic;
using Orchard;
using Transformalize.Orchard.Models;

namespace Transformalize.Orchard.Services {
    public interface IApiService : IDependency {
        ApiResponse NotFound(ApiRequest request);
        ApiResponse Unathorized(ApiRequest request);
        List<ApiResponse> Rejections(int id, out ApiRequest request, out ConfigurationPart part);
    }
}