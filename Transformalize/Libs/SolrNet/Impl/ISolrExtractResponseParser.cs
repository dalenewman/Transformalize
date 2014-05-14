using System.Xml.Linq;

namespace Transformalize.Libs.SolrNet.Impl {
    /// <summary>
    /// Parses the extract response
    /// </summary>
    public interface ISolrExtractResponseParser {
        ExtractResponse Parse(XDocument response);
    }
}