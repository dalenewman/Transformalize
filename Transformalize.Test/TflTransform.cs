using Transformalize.Libs.NanoXml;
using Transformalize.Main;

namespace Transformalize.Test
{
    public class TflTransform : TflNode {
        public TflTransform(NanoXmlNode node)
            : base(node) {
            Property("after-aggregation", true);
            Property("before-aggregation", false);
            Property("branches", string.Empty);
            Property("characters", string.Empty);
            Property("connection", string.Empty);
            Property("contains-characters", "All");
            Property("content-type", string.Empty);
            Property("count", 0);
            Property("data", Common.DefaultValue);
            Property("decode", false);
            Property("domain", string.Empty);
            Property("elipse", "...");
            Property("else", string.Empty);
            Property("encode", false);
            Property("encoding", Common.DefaultValue);
            Property("fields", string.Empty);
            Property("format", string.Empty);
            Property("from-lat", "0.0");
            Property("from-long", "0.0");
            Property("from-time-zone", string.Empty);
            Property("ignore-empty", false);
            Property("index", 0);
            Property("interval", 0);
            Property("left", string.Empty);
            Property("length", 0);
            Property("lower-bound", false);
            Property("lower-bound-type", "Inclusive");
            Property("lower-unit", "None");
            Property("map", string.Empty);
            Property("message-append", false);
            Property("message-field", Common.DefaultValue);
            Property("message-template", string.Empty);
            Property("method", string.Empty);
            Property("model", "dynamic");
            Property("name", string.Empty);
            Property("negated", false);
            Property("new-value", string.Empty);
            Property("old-value", string.Empty);
            Property("operator", "Equal");
            Property("padding-char", "0");
            Property("parameter", string.Empty);
            Property("parameters", string.Empty);
            Property("pattern", string.Empty);
            Property("replacement", string.Empty);
            Property("replace-single-quotes", true);
            Property("result-field", Common.DefaultValue);
            Property("right", string.Empty);
            Property("root", string.Empty);
            Property("run-field", string.Empty);
            Property("run-operator", "Equal");
            Property("run-type", Common.DefaultValue);
            Property("run-value", Common.DefaultValue);
            Property("script", string.Empty);
            Property("scripts", string.Empty);
            Property("separator", Common.DefaultValue);
            Property("sleep", 0);
            Property("start-index", 0);
            Property("t", string.Empty);
            Property("tag", string.Empty);
            Property("target-field", string.Empty);
            Property("template", string.Empty);
            Property("templates", string.Empty);
            Property("then", string.Empty);
            Property("time-component", "milliseconds");
            Property("time-out", 0);
            Property("to", string.Empty);
            Property("to-lat", "0.0");
            Property("to-long", "0.0");
            Property("total-width", 0);
            Property("to-time-zone", string.Empty);
            Property("trim-chars", " ");
            Property("type", string.Empty);
            Property("units", "meters");
            Property("upper-bound", false);
            Property("upper-bound-type", "Inclusive");
            Property("upper-unit", "None");
            Property("url", string.Empty);
            Property("use-https", false);
            Property("value", string.Empty);
            Property("web-method", "GET");
            Property("xml-mode", "Default");
            Property("xpath", string.Empty);

            Class<TflParameter>("parameters");
            Class<TflNameReference>("scripts");
            Class<TflNameReference>("templates");
            Class<TflBranch>("branches");
            Class<TflField>("fields");
            }
    }
}