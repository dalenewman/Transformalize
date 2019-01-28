#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Cfg.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize.Configuration {

    public class Operation : CfgNode {

        public const string TransformProducerDomain = "now,next,last,connection";
        public const string TimeZoneIdDomain = "Dateline Standard Time,UTC-11,Samoa Standard Time,Hawaiian Standard Time,Alaskan Standard Time,Pacific Standard Time (Mexico),Pacific Standard Time,US Mountain Standard Time,Mountain Standard Time (Mexico),Mountain Standard Time,Central America Standard Time,Central Standard Time,Central Standard Time (Mexico),Canada Central Standard Time,SA Pacific Standard Time,Eastern Standard Time,US Eastern Standard Time,Venezuela Standard Time,Paraguay Standard Time,Atlantic Standard Time,Central Brazilian Standard Time,SA Western Standard Time,Pacific SA Standard Time,Newfoundland Standard Time,E. South America Standard Time,Argentina Standard Time,SA Eastern Standard Time,Greenland Standard Time,Montevideo Standard Time,UTC-02,Mid-Atlantic Standard Time,Azores Standard Time,Cape Verde Standard Time,Morocco Standard Time,UTC,GMT Standard Time,Greenwich Standard Time,W. Europe Standard Time,Central Europe Standard Time,Romance Standard Time,Central European Standard Time,W. Central Africa Standard Time,Namibia Standard Time,Jordan Standard Time,GTB Standard Time,Middle East Standard Time,Egypt Standard Time,Syria Standard Time,South Africa Standard Time,FLE Standard Time,Israel Standard Time,E. Europe Standard Time,Arabic Standard Time,Arab Standard Time,Russian Standard Time,E. Africa Standard Time,Iran Standard Time,Arabian Standard Time,Azerbaijan Standard Time,Mauritius Standard Time,Georgian Standard Time,Caucasus Standard Time,Afghanistan Standard Time,Ekaterinburg Standard Time,Pakistan Standard Time,West Asia Standard Time,India Standard Time,Sri Lanka Standard Time,Nepal Standard Time,Central Asia Standard Time,Bangladesh Standard Time,N. Central Asia Standard Time,Myanmar Standard Time,SE Asia Standard Time,North Asia Standard Time,China Standard Time,North Asia East Standard Time,Singapore Standard Time,W. Australia Standard Time,Taipei Standard Time,Ulaanbaatar Standard Time,Tokyo Standard Time,Korea Standard Time,Yakutsk Standard Time,Cen. Australia Standard Time,AUS Central Standard Time,E. Australia Standard Time,AUS Eastern Standard Time,West Pacific Standard Time,Tasmania Standard Time,Vladivostok Standard Time,Central Pacific Standard Time,New Zealand Standard Time,UTC+12,Fiji Standard Time,Kamchatka Standard Time,Tonga Standard Time";
        public const string DayOfWeekDomain = "sunday,monday,tuesday,wednesday,thursday,friday,saturday";

        private static HashSet<string> _transformProducerSet;
        private string _runOperator;

        public bool ProducesFields { get; private set; }

        [Cfg(value = "")]
        public string Connection { get; set; }

        [Cfg(value = "")]
        public string ContentType { get; set; }

        [Cfg(value = 0)]
        public int Count { get; set; }

        [Cfg(value = "")]
        public string Domain { get; set; }

        [Cfg(value = false)]
        public bool Encode { get; set; }

        [Cfg(value = "")]
        public string Format { get; set; }

        [Cfg(value = Constants.DefaultSetting)]
        public string FromLat { get; set; }

        [Cfg(value = Constants.DefaultSetting)]
        public string FromLon { get; set; }

        [Cfg(value = Constants.DefaultSetting, domain = Constants.DefaultSetting + "," + TimeZoneIdDomain)]
        public string FromTimeZone { get; set; }

        [Cfg(value = 0)]
        public int Index { get; set; }

        [Cfg(value = 0)]
        public int Length { get; set; }

        [Cfg(value = "", toLower = true)]
        public string Map { get; set; }

        [Cfg(required = true, toLower = true)]
        public string Method { get; set; }

        [Cfg(value = "")]
        public string Name { get; set; }

        [Cfg(value = false)]
        public bool Negated { get; set; }

        [Cfg(value = "")]
        public string NewValue { get; set; }

        [Cfg(value = "")]
        public string OldValue { get; set; }

        [Cfg(value = "equal", domain = Constants.ComparisonDomain, toLower = true, ignoreCase = true)]
        public string Operator { get; set; }

        [Cfg(value = '0')]
        public char PaddingChar { get; set; }

        [Cfg(value = "")]
        public string Parameter { get; set; }

        [Cfg(value = "")]
        public string Pattern { get; set; }

        [Cfg(value = "")]
        public string Root { get; set; }

        [Cfg(value = "")]
        public string RunField { get; set; }

        [Cfg(value = "equal", domain = Constants.ComparisonDomain, toLower = true)]
        public string RunOperator {
            get => _runOperator;
            set {
                value = value?.TrimEnd('s');
                _runOperator = value;
            }
        }

        [Cfg(value = Constants.DefaultSetting)]
        public string RunValue { get; set; }

        [Cfg(value = "")]
        public string Script { get; set; }

        [Cfg(value = Constants.DefaultSetting)]
        public string Separator { get; set; }

        [Cfg(value = 0)]
        public int StartIndex { get; set; }

        [Cfg(value = "")]
        public string Tag { get; set; }

        [Cfg(value = "")]
        public string HRef { get; set; }

        [Cfg(value = "")]
        public string Class { get; set; }

        [Cfg(value = "")]
        public string Title { get; set; }

        [Cfg(value = "")]
        public string Style { get; set; }

        [Cfg(value = "")]
        public string Role { get; set; }

        [Cfg(value = "")]
        public string Target { get; set; }

        [Cfg(value = "")]
        public string Body { get; set; }

        [Cfg(value = "")]
        public string Src { get; set; }

        [Cfg(value = 0)]
        public int Width { get; set; }

        [Cfg(value = 0)]
        public int Height { get; set; }

        [Cfg(value = "")]
        public string Template { get; set; }

        [Cfg(value = "millisecond", domain = "day,days,date,dayofweek,dayofyear,hour,hours,millisecond,milliseconds,ms,minute,minutes,month,months,second,seconds,s,tick,ticks,year,years,weekofyear", toLower = true)]
        public string TimeComponent { get; set; }

        [Cfg(value = "0.0")]
        public string ToLat { get; set; }

        [Cfg(value = "0.0")]
        public string ToLon { get; set; }

        [Cfg(value = 0)]
        public int TotalWidth { get; set; }

        [Cfg(value = Constants.DefaultSetting, domain = Constants.DefaultSetting + "," + TimeZoneIdDomain)]
        public string ToTimeZone { get; set; }

        [Cfg(value = " ")]
        public string TrimChars { get; set; }

        [Cfg(value = Constants.DefaultSetting, domain = Constants.DefaultSetting + "," + Constants.TypeDomain)]
        public string Type { get; set; }

        [Cfg(value = "meters,bits,b,bytes,kb,kilobytes,mb,megabytes,gb,gigabytes,tb,terabytes", toLower = true, ignoreCase = true)]
        public string Units { get; set; }

        [Cfg(value = "")]
        public string Url { get; set; }

        [Cfg(value = Constants.DefaultSetting)]
        public string Value { get; set; }

        [Cfg(value = "GET", domain = "GET,POST", ignoreCase = true, toUpper = true)]
        public string WebMethod { get; set; }

        [Cfg(value = "first", domain = "first,all", ignoreCase = true, toLower = true)]
        public string Mode { get; set; }

        [Obsolete("Use Mode instead.")]
        [Cfg(value = "first", domain = "first,all", ignoreCase = true, toLower = true)]
        public string XmlMode { get; set; }

        [Cfg]
        public List<Parameter> Parameters { get; set; }

        [Cfg]
        public List<NameReference> Scripts { get; set; }

        [Cfg]
        public List<Field> Fields { get; set; }

        [Cfg(value = "firstday", domain = "firstday,firstfourdayweek,firstfullweek", toLower = true)]
        public string CalendarWeekRule { get; set; }

        [Cfg(domain = DayOfWeekDomain, toLower = true)]
        public string DayOfWeek { get; set; }

        [Cfg]
        public string Property { get; set; }

        public Func<IRow, bool> ShouldRun { get; set; }

        [Cfg(value = true)]
        public bool Extension { get; set; }

        [Cfg(value = "")]
        public string NameSpace { get; set; }

        [Cfg(value = 0)]
        public int Decimals { get; set; }

        public string Returns { get; set; }

        [Cfg(value = "", trim = true)]
        public string Expression { get; set; }

        [Cfg(value = "", trim = true)]
        public string TrueField { get; set; }

        [Cfg(value = "", trim = true)]
        public string FalseField { get; set; }

        [Cfg(value = "")]
        public string Latitude { get; set; }

        [Cfg(value = "")]
        public string Longitude { get; set; }

        [Cfg(value = "north", domain = "north,northeast,east,southeast,south,southwest,west,northwest,asc,desc", toLower = true, ignoreCase = true, trim = true)]
        public string Direction { get; set; }

        public string Key { get; set; } = string.Empty;

        [Cfg(value = "")]
        public string ApiKey { get; set; }

        [Cfg(value = 50)]
        public int Limit { get; set; }

        [Cfg(value = "")]
        public string AdministrativeArea { get; set; }

        [Cfg(value = "")]
        public string Route { get; set; }

        [Cfg(value = "")]
        public string Country { get; set; }

        [Cfg(value = "")]
        public string Locality { get; set; }

        [Cfg(value = "")]
        public string PostalCode { get; set; }

        [Cfg(value = 1000)]
        public int Time { get; set; }

        [Cfg(value = "info", toLower = true, ignoreCase = true, domain = "info,error,debug,warn")]
        public string Level { get; set; }

        [Cfg(value = 0)]
        public int Seed { get; set; }

        [Cfg(value = 1)]
        public int Step { get; set; }

        [Cfg(value = "")]
        public string Query { get; set; }

        [Cfg(value = "")]
        public string Command { get; set; }

        public static HashSet<string> TransformProducerSet() {
            return _transformProducerSet ?? (_transformProducerSet = new HashSet<string>(TransformProducerDomain.Split(',')));
        }

        public override string ToString() {
            return Method;
        }

        protected override void PreValidate() {
            // while XmlMode is still available
            if (XmlMode == "all" && Mode == "first") {
                Mode = "all";
                Warn("XmlMode is being phased out.  Please use Mode instead.");
            }
        }

        protected override void Validate() {
            if (Fields.Any()) {
                ProducesFields = true;
            }
        }

    }
}