using System.Collections.Generic;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class TflTransform : CfgNode {

        public TflTransform() {

            Property(n: "after-aggregation", v: true);
            Property(n: "before-aggregation", v: false);
            Property(n: "characters", v: string.Empty);
            Property(n: "connection", v: string.Empty);
            Property(n: "contains-characters", v: "All");
            Property(n: "content-type", v: string.Empty);
            Property(n: "count", v: 0);
            Property(n: "data", v: Common.DefaultValue);
            Property(n: "decode", v: false);
            Property(n: "domain", v: string.Empty);
            Property(n: "elipse", v: "...");
            Property(n: "else", v: string.Empty);
            Property(n: "encode", v: false);
            Property(n: "encoding", v: Common.DefaultValue);
            Property(n: "format", v: string.Empty);
            Property(n: "from-lat", v: "0.0");
            Property(n: "from-long", v: "0.0");
            Property(n: "from-time-zone", v: string.Empty);
            Property(n: "ignore-empty", v: false);
            Property(n: "index", v: 0);
            Property(n: "interval", v: 0);
            Property(n: "left", v: string.Empty);
            Property(n: "length", v: 0);
            Property(n: "lower-bound", v: false);
            Property(n: "lower-bound-type", v: "Inclusive");
            Property(n: "lower-unit", v: "None");
            Property(n: "map", v: string.Empty);
            Property(n: "message-append", v: false);
            Property(n: "message-field", v: Common.DefaultValue);
            Property(n: "message-template", v: string.Empty);
            Property(n: "method", v: string.Empty);
            Property(n: "model", v: "dynamic");
            Property(n: "name", v: string.Empty);
            Property(n: "negated", v: false);
            Property(n: "new-value", v: string.Empty);
            Property(n: "old-value", v: string.Empty);
            Property(n: "operator", v: "Equal");
            Property(n: "padding-char", v: "0");
            Property(n: "parameter", v: string.Empty);
            Property(n: "pattern", v: string.Empty);
            Property(n: "replacement", v: string.Empty);
            Property(n: "replace-single-quotes", v: true);
            Property(n: "result-field", v: Common.DefaultValue);
            Property(n: "right", v: string.Empty);
            Property(n: "root", v: string.Empty);
            Property(n: "run-field", v: string.Empty);
            Property(n: "run-operator", v: "Equal");
            Property(n: "run-type", v: Common.DefaultValue);
            Property(n: "run-value", v: Common.DefaultValue);
            Property(n: "script", v: string.Empty);
            Property(n: "separator", v: Common.DefaultValue);
            Property(n: "sleep", v: 0);
            Property(n: "start-index", v: 0);
            Property(n: "t", v: string.Empty);
            Property(n: "tag", v: string.Empty);
            Property(n: "target-field", v: string.Empty);
            Property(n: "template", v: string.Empty);
            Property(n: "then", v: string.Empty);
            Property(n: "time-component", v: "milliseconds");
            Property(n: "time-out", v: 0);
            Property(n: "to", v: string.Empty);
            Property(n: "to-lat", v: "0.0");
            Property(n: "to-long", v: "0.0");
            Property(n: "total-width", v: 0);
            Property(n: "to-time-zone", v: string.Empty);
            Property(n: "trim-chars", v: " ");
            Property(n: "type", v: string.Empty);
            Property(n: "units", v: "meters");
            Property(n: "upper-bound", v: false);
            Property(n: "upper-bound-type", v: "Inclusive");
            Property(n: "upper-unit", v: "None");
            Property(n: "url", v: string.Empty);
            Property(n: "use-https", v: false);
            Property(n: "value", v: string.Empty);
            Property(n: "web-method", v: "GET");
            Property(n: "xml-mode", v: "Default");
            Property(n: "xpath", v: string.Empty);

            Class<TflParameter>("parameters");
            Class<TflNameReference>("scripts");
            Class<TflNameReference>("templates");
            Class<TflBranch>("branches");
            Class<TflField>("fields");
        }

        public bool AfterAggregation { get; set; }
        public bool BeforeAggregation { get; set; }
        public string Characters { get; set; }
        public string Connection { get; set; }
        public string ContainsCharacters { get; set; }
        public string ContentType { get; set; }
        public int Count { get; set; }
        public string Data { get; set; }
        public bool Decode { get; set; }
        public string Domain { get; set; }
        public string Elipse { get; set; }
        public string Else { get; set; }
        public bool Encode { get; set; }
        public string Encoding { get; set; }
        public string Format { get; set; }
        public string FromLat { get; set; }
        public string FromLong { get; set; }
        public string FromTimeZone { get; set; }
        public bool IgnoreEmpty { get; set; }
        public int Index { get; set; }
        public int Interval { get; set; }
        public string Left { get; set; }
        public int Length { get; set; }
        public bool LowerBound { get; set; }
        public string LowerBoundType { get; set; }
        public string LowerUnit { get; set; }
        public string Map { get; set; }
        public bool MessageAppend { get; set; }
        public string MessageField { get; set; }
        public string MessageTemplate { get; set; }
        public string Method { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public bool Negated { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string Operator { get; set; }
        public string PaddingChar { get; set; }
        public string Parameter { get; set; }
        public string Pattern { get; set; }
        public string Replacement { get; set; }
        public bool ReplaceSingleQuotes { get; set; }
        public string ResultField { get; set; }
        public string Right { get; set; }
        public string Root { get; set; }
        public string RunField { get; set; }
        public string RunOperator { get; set; }
        public string RunType { get; set; }
        public string RunValue { get; set; }
        public string Script { get; set; }
        public string Separator { get; set; }
        public int Sleep { get; set; }
        public int StartIndex { get; set; }
        public string T { get; set; }
        public string Tag { get; set; }
        public string TargetField { get; set; }
        public string Template { get; set; }
        public string Then { get; set; }
        public string TimeComponent { get; set; }
        public int TimeOut { get; set; }
        public string To { get; set; }
        public string ToLat { get; set; }
        public string ToLong { get; set; }
        public int TotalWidth { get; set; }
        public string ToTimeZone { get; set; }
        public string TrimChars { get; set; }
        public string Type { get; set; }
        public string Units { get; set; }
        public bool UpperBound { get; set; }
        public string UpperBoundType { get; set; }
        public string UpperUnit { get; set; }
        public string Url { get; set; }
        public bool UseHttps { get; set; }
        public string Value { get; set; }
        public string WebMethod { get; set; }
        public string XmlMode { get; set; }
        public string Xpath { get; set; }

        public List<TflParameter> Parameters { get; set; }
        public List<TflNameReference> Scripts { get; set; }
        public List<TflNameReference> Templates { get; set; }
        public List<TflBranch> Branches { get; set; }
        public List<TflField> Fields { get; set; } 
    }
}