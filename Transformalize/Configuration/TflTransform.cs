using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class TflTransform : CfgNode {

        public TflTransform() {

            Property(name: "after-aggregation", value: true);
            Property(name: "before-aggregation", value: false);
            Property(name: "characters", value: string.Empty);
            Property(name: "connection", value: string.Empty);
            Property(name: "contains-characters", value: "All");
            Property(name: "content-type", value: string.Empty);
            Property(name: "count", value: 0);
            Property(name: "data", value: Common.DefaultValue);
            Property(name: "decode", value: false);
            Property(name: "domain", value: string.Empty);
            Property(name: "elipse", value: "...");
            Property(name: "else", value: string.Empty);
            Property(name: "encode", value: false);
            Property(name: "encoding", value: Common.DefaultValue);
            Property(name: "format", value: string.Empty);
            Property(name: "from-lat", value: "0.0");
            Property(name: "from-long", value: "0.0");
            Property(name: "from-time-zone", value: string.Empty);
            Property(name: "ignore-empty", value: false);
            Property(name: "index", value: 0);
            Property(name: "interval", value: 0);
            Property(name: "left", value: string.Empty);
            Property(name: "length", value: 0);
            Property(name: "lower-bound", value: false);
            Property(name: "lower-bound-type", value: "Inclusive");
            Property(name: "lower-unit", value: "None");
            Property(name: "map", value: string.Empty);
            Property(name: "message-append", value: false);
            Property(name: "message-field", value: Common.DefaultValue);
            Property(name: "message-template", value: string.Empty);
            Property(name: "method", value: string.Empty);
            Property(name: "model", value: "dynamic");
            Property(name: "name", value: string.Empty);
            Property(name: "negated", value: false);
            Property(name: "new-value", value: string.Empty);
            Property(name: "old-value", value: string.Empty);
            Property(name: "operator", value: "Equal");
            Property(name: "padding-char", value: "0");
            Property(name: "parameter", value: string.Empty);
            Property(name: "pattern", value: string.Empty);
            Property(name: "replacement", value: string.Empty);
            Property(name: "replace-single-quotes", value: true);
            Property(name: "result-field", value: Common.DefaultValue);
            Property(name: "right", value: string.Empty);
            Property(name: "root", value: string.Empty);
            Property(name: "run-field", value: string.Empty);
            Property(name: "run-operator", value: "Equal");
            Property(name: "run-type", value: Common.DefaultValue);
            Property(name: "run-value", value: Common.DefaultValue);
            Property(name: "script", value: string.Empty);
            Property(name: "separator", value: Common.DefaultValue);
            Property(name: "sleep", value: 0);
            Property(name: "start-index", value: 0);
            Property(name: "t", value: string.Empty);
            Property(name: "tag", value: string.Empty);
            Property(name: "target-field", value: string.Empty);
            Property(name: "template", value: string.Empty);
            Property(name: "then", value: string.Empty);
            Property(name: "time-component", value: "milliseconds");
            Property(name: "time-out", value: 0);
            Property(name: "to", value: string.Empty);
            Property(name: "to-lat", value: "0.0");
            Property(name: "to-long", value: "0.0");
            Property(name: "total-width", value: 0);
            Property(name: "to-time-zone", value: string.Empty);
            Property(name: "trim-chars", value: " ");
            Property(name: "type", value: string.Empty);
            Property(name: "units", value: "meters");
            Property(name: "upper-bound", value: false);
            Property(name: "upper-bound-type", value: "Inclusive");
            Property(name: "upper-unit", value: "None");
            Property(name: "url", value: string.Empty);
            Property(name: "use-https", value: false);
            Property(name: "value", value: string.Empty);
            Property(name: "web-method", value: "GET");
            Property(name: "xml-mode", value: "Default");
            Property(name: "xpath", value: string.Empty);

            Collection<TflParameter>("parameters");
            Collection<TflNameReference>("scripts");
            Collection<TflNameReference>("templates");
            Collection<TflBranch>("branches");
            Collection<TflField>("fields");
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