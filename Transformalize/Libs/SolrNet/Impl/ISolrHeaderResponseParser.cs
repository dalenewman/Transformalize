using System.Xml.Linq;

namespace Transformalize.Libs.SolrNet.Impl {
    public interface ISolrHeaderResponseParser {
        ResponseHeader Parse(XDocument response);
    }
}