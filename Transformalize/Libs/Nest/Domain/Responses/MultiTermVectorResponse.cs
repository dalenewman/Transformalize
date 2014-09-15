using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Responses
{
    public interface IMultiTermVectorResponse : IResponse
    {
        IEnumerable<TermVectorResponse> Documents { get; }
    }

    [JsonObject]
    public class MultiTermVectorResponse : BaseResponse, IMultiTermVectorResponse
    {
        public MultiTermVectorResponse()
        {
            IsValid = true;
            Documents = new List<TermVectorResponse>();
        }

        [JsonProperty("docs")]
        public IEnumerable<TermVectorResponse> Documents { get; internal set; }
    }
}
