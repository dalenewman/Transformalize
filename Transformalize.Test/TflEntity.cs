using Transformalize.Libs.NanoXml;

namespace Transformalize.Test
{
    public class TflEntity : TflNode {
        public TflEntity(NanoXmlNode node)
            : base(node) {
            Property("alias", string.Empty, false, true);  // should be unique, if entered
            Property("connection", "input");
            Property("delete", false);
            Property("detect-changes", true);
            Property("group", false);
            Property("name", string.Empty, true);
            Property("no-lock", false);
            Property("pipeline-threading", "Default");
            Property("prefix", string.Empty);
            Property("prepend-process-name-to-output-name", true);
            Property("sample", 100);
            Property("schema", string.Empty);
            Property("query-keys", string.Empty);
            Property("query", string.Empty);
            Property("script-keys", string.Empty);
            Property("script", string.Empty);
            Property("trim-all", false);
            Property("unicode", string.Empty);
            Property("variable-length", string.Empty);
            Property("version", string.Empty);

            Class<TflFilter>("filter");
            Class<TflField>("fields");
            Class<TflCalculatedField>("calculated-fields");
            Class<TflIo>("input");
            Class<TflIo>("output");
            }
    }
}