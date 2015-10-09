using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cfg.Net;
using Transformalize.Libs.Jint.Parser;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class TflTransform : CfgNode {

        [Cfg(value = true)]
        public bool AfterAggregation { get; set; }
        [Cfg(value = false)]
        public bool BeforeAggregation { get; set; }
        [Cfg(value = "")]
        public string Characters { get; set; }
        [Cfg(value = "")]
        public string Connection { get; set; }
        [Cfg(value = "All", domain = "All,Any")]
        public string ContainsCharacters { get; set; }
        [Cfg(value = "")]
        public string ContentType { get; set; }
        [Cfg(value = 0)]
        public int Count { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Data { get; set; }
        [Cfg(value = false)]
        public bool Decode { get; set; }
        [Cfg(value = "")]
        public string Domain { get; set; }
        [Cfg(value = "...")]
        public string Elipse { get; set; }
        [Cfg(value = "")]
        public string Else { get; set; }
        [Cfg(value = false)]
        public bool Encode { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Encoding { get; set; }
        [Cfg(value = "")]
        public string Format { get; set; }
        [Cfg(value = "0.0")]
        public string FromLat { get; set; }
        [Cfg(value = "0.0")]
        public string FromLong { get; set; }
        [Cfg(value = "")]
        public string FromTimeZone { get; set; }
        [Cfg(value = false)]
        public bool IgnoreEmpty { get; set; }
        [Cfg(value = 0)]
        public int Index { get; set; }
        [Cfg(value = 0)]
        public int Interval { get; set; }
        [Cfg(value = "")]
        public string Left { get; set; }
        [Cfg(value = 0)]
        public int Length { get; set; }
        [Cfg(value = null)]
        public object LowerBound { get; set; }
        [Cfg(value = "Inclusive", domain = "Inclusive,Exclusive,Ignore")]
        public string LowerBoundType { get; set; }
        [Cfg(value = "None")]
        public string LowerUnit { get; set; }
        [Cfg(value = "")]
        public string Map { get; set; }

        [Cfg(required = true, toLower = true)]
        public string Method { get; set; }

        [Cfg(value = "dynamic")]
        public string Model { get; set; }
        [Cfg(value = "")]
        public string Name { get; set; }
        [Cfg(value = false)]
        public bool Negated { get; set; }
        [Cfg(value = "")]
        public string NewValue { get; set; }
        [Cfg(value = "")]
        public string OldValue { get; set; }
        [Cfg(value = "Equal", domain = Common.ValidComparisons)]
        public string Operator { get; set; }
        [Cfg(value = '0')]
        public char PaddingChar { get; set; }
        [Cfg(value = "")]
        public string Parameter { get; set; }
        [Cfg(value = "")]
        public string Pattern { get; set; }
        [Cfg(value = "")]
        public string Replacement { get; set; }
        [Cfg(value = true)]
        public bool ReplaceSingleQuotes { get; set; }
        [Cfg(value = "")]
        public string Right { get; set; }
        [Cfg(value = "")]
        public string Root { get; set; }
        [Cfg(value = "")]
        public string RunField { get; set; }
        [Cfg(value = "Equal", domain = Common.ValidComparisons)]
        public string RunOperator { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string RunType { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string RunValue { get; set; }
        [Cfg(value = "")]
        public string Script { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string Separator { get; set; }
        [Cfg(value = 0)]
        public int Sleep { get; set; }
        [Cfg(value = 0)]
        public int StartIndex { get; set; }
        [Cfg(value = "")]
        public string T { get; set; }
        [Cfg(value = "")]
        public string Tag { get; set; }
        [Cfg(value = "")]
        public string TargetField { get; set; }
        [Cfg(value = "")]
        public string Template { get; set; }
        [Cfg(value = "")]
        public string Then { get; set; }
        [Cfg(value = "milliseconds")]
        public string TimeComponent { get; set; }
        [Cfg(value = 0)]
        public int TimeOut { get; set; }
        [Cfg(value = Common.DefaultValue, domain = Common.DefaultValue + "," + Common.ValidTypes)]
        public string To { get; set; }
        [Cfg(value = "0.0")]
        public string ToLat { get; set; }
        [Cfg(value = "0.0")]
        public string ToLong { get; set; }
        [Cfg(value = 0)]
        public int TotalWidth { get; set; }
        [Cfg(value = "")]
        public string ToTimeZone { get; set; }
        [Cfg(value = " ")]
        public string TrimChars { get; set; }
        [Cfg(value = "")]
        public string Type { get; set; }
        [Cfg(value = "meters")]
        public string Units { get; set; }
        [Cfg(value = null)]
        public object UpperBound { get; set; }
        [Cfg(value = "Inclusive", domain = "Inclusive,Exclusive,Ignore", ignoreCase = true)]
        public string UpperBoundType { get; set; }
        [Cfg(value = "None")]
        public string UpperUnit { get; set; }
        [Cfg(value = "")]
        public string Url { get; set; }
        [Cfg(value = false)]
        public bool UseHttps { get; set; }
        [Cfg(value = "")]
        public string Value { get; set; }
        [Cfg(value = "GET", domain = "GET,POST", ignoreCase = true)]
        public string WebMethod { get; set; }
        [Cfg(value = "Default")]
        public string XmlMode { get; set; }
        [Cfg(value = "")]
        public string Xpath { get; set; }

        [Cfg()]
        public List<TflParameter> Parameters { get; set; }
        [Cfg()]
        public List<TflNameReference> Scripts { get; set; }
        [Cfg()]
        public List<TflNameReference> Templates { get; set; }
        [Cfg()]
        public List<TflBranch> Branches { get; set; }
        [Cfg()]
        public List<TflField> Fields { get; set; }

        public bool IsShortHand { get; set; }

        [Cfg(value = "firstday", domain = "firstday,firstfourdayweek,firstfullweek", toLower = true)]
        public string CalendarWeekRule { get; set; }

        [Cfg(value = "sunday", domain = "friday,monday,saturday,sunday,tuesday,thursday,wednesday", toLower = true)]
        public string DayOfWeek { get; set; }

        protected override void PreValidate() {
            switch (Method) {
                case "trimstartappend":
                    if (Separator.Equals(Common.DefaultValue)) {
                        Separator = " ";
                    }
                    break;
            }
        }

        protected override void Validate() {

            switch (Method) {
                case "shorthand":
                    if (string.IsNullOrEmpty(T)) {
                        Error("shorthand transform requires t attribute.");
                    }
                    break;
                case "copy":
                    if (Parameter == string.Empty && !Parameters.Any()) {
                        Error("copy transform requires a parameter (or parameters).");
                    }
                    break;
                case "javascript":
                    ValidateJavascript();
                    break;
            }
        }

        private void ValidateJavascript() {
            //TODO: extract interface and inject parser
            try {
                var program = new JavaScriptParser().Parse(Script, new ParserOptions() { Tolerant = true });
                if (program.Errors == null)
                    return;
                foreach (var parserError in program.Errors) {
                    Error("Javascript parse error. {0}", parserError.Message);
                }
            } catch (Exception ex) {
                Error("Javascript parse error. {0}", ex.Message);
            }
        }

    }
}