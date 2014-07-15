using System;
using System.Collections.Specialized;
using System.Text;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Runner {
    public abstract class ContentsReader : WithLoggingMixin {

        public abstract Contents Read(string resource);

        protected string ReplaceParameters(string content, NameValueCollection query) {
            if (query == null || !query.HasKeys())
                return content;

            var builder = new StringBuilder(content);
            foreach (var key in query.AllKeys) {
                builder.Replace(string.Format("@({0})", key), XmlEncodeOperation.SanitizeXmlString(query[key]));
                Debug("Replaced {0} with {1} per url's query string.", "@" + key, query[key]);
            }
            return builder.ToString();
        }
    }
}