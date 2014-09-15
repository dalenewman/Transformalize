using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Responses
{
    public interface ITermVectorResponse : IResponse
    {
        bool Found { get; }
        IDictionary<string, TermVector.TermVector> TermVectors { get; }
    }

    [JsonObject]
    public class TermVectorResponse : BaseResponse, ITermVectorResponse
    {
        public TermVectorResponse()
        {
            IsValid = true;
            TermVectors = new Dictionary<string, TermVector.TermVector>();
        }

        [JsonProperty("found")]
        public bool Found { get; internal set; }

        [JsonProperty("term_vectors")]
        public IDictionary<string, TermVector.TermVector> TermVectors { get; internal set; }
    }
}
