#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System;
using System.Collections.Generic;
using Cfg.Net;
using Pipeline.Contracts;

namespace Pipeline.Configuration {

    public class Transform : CfgNode {

        public const string ProducerDomain = "fromxml,fromsplit";
        public const string TransformerDomain = "now,utcnow,concat,copy,format,hashcode,htmldecode,left,right,xmldecode,padleft,padright,splitlength,trim,trimstart,trimend,javascript,js,tostring,toupper,upper,tolower,lower,join,map,decompress,timezone,next,last,substring,datepart,toyesno,regexreplace,formatphone,replace,remove,insert,cs,csharp,timeago,timeahead,convert,totime,razor,any,connection,filename,fileext,filepath,xpath";
        public const string TransformProducerDomain = "now,utcnow,next,last,connection";
        public const string ValidatorDomain = "contains,is";
        public const string TimeZoneIdDomain = "Dateline Standard Time,UTC-11,Samoa Standard Time,Hawaiian Standard Time,Alaskan Standard Time,Pacific Standard Time (Mexico),Pacific Standard Time,US Mountain Standard Time,Mountain Standard Time (Mexico),Mountain Standard Time,Central America Standard Time,Central Standard Time,Central Standard Time (Mexico),Canada Central Standard Time,SA Pacific Standard Time,Eastern Standard Time,US Eastern Standard Time,Venezuela Standard Time,Paraguay Standard Time,Atlantic Standard Time,Central Brazilian Standard Time,SA Western Standard Time,Pacific SA Standard Time,Newfoundland Standard Time,E. South America Standard Time,Argentina Standard Time,SA Eastern Standard Time,Greenland Standard Time,Montevideo Standard Time,UTC-02,Mid-Atlantic Standard Time,Azores Standard Time,Cape Verde Standard Time,Morocco Standard Time,UTC,GMT Standard Time,Greenwich Standard Time,W. Europe Standard Time,Central Europe Standard Time,Romance Standard Time,Central European Standard Time,W. Central Africa Standard Time,Namibia Standard Time,Jordan Standard Time,GTB Standard Time,Middle East Standard Time,Egypt Standard Time,Syria Standard Time,South Africa Standard Time,FLE Standard Time,Israel Standard Time,E. Europe Standard Time,Arabic Standard Time,Arab Standard Time,Russian Standard Time,E. Africa Standard Time,Iran Standard Time,Arabian Standard Time,Azerbaijan Standard Time,Mauritius Standard Time,Georgian Standard Time,Caucasus Standard Time,Afghanistan Standard Time,Ekaterinburg Standard Time,Pakistan Standard Time,West Asia Standard Time,India Standard Time,Sri Lanka Standard Time,Nepal Standard Time,Central Asia Standard Time,Bangladesh Standard Time,N. Central Asia Standard Time,Myanmar Standard Time,SE Asia Standard Time,North Asia Standard Time,China Standard Time,North Asia East Standard Time,Singapore Standard Time,W. Australia Standard Time,Taipei Standard Time,Ulaanbaatar Standard Time,Tokyo Standard Time,Korea Standard Time,Yakutsk Standard Time,Cen. Australia Standard Time,AUS Central Standard Time,E. Australia Standard Time,AUS Eastern Standard Time,West Pacific Standard Time,Tasmania Standard Time,Vladivostok Standard Time,Central Pacific Standard Time,New Zealand Standard Time,UTC+12,Fiji Standard Time,Kamchatka Standard Time,Tonga Standard Time";
        public const string DayOfWeekDomain = "sunday,monday,tuesday,wednesday,thursday,friday,saturday";

        static HashSet<string> _transformSet;
        static HashSet<string> _transformProducerSet;
        static HashSet<string> _validateSet;
        static HashSet<string> _producerSet;
        private string _runOperator;

        public IParser Parser { get; set; }

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
        [Cfg(value = Constants.DefaultSetting)]
        public string Data { get; set; }
        [Cfg(value = "")]
        public string Domain { get; set; }
        [Cfg(value = "...")]
        public string Elipse { get; set; }
        [Cfg(value = "")]
        public string Else { get; set; }
        [Cfg(value = false)]
        public bool Encode { get; set; }
        [Cfg(value = Constants.DefaultSetting)]
        public string Encoding { get; set; }
        [Cfg(value = "")]
        public string Format { get; set; }
        [Cfg(value = "0.0")]
        public string FromLat { get; set; }
        [Cfg(value = "0.0")]
        public string FromLong { get; set; }

        [Cfg(value = Constants.DefaultSetting, domain = Constants.DefaultSetting + "," + TimeZoneIdDomain)]
        public string FromTimeZone { get; set; }
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
        [Cfg(value = "", toLower = true)]
        public string Map { get; set; }

        [Cfg(required = true, toLower = true, domain = TransformerDomain + "," + ValidatorDomain + "," + ProducerDomain)]
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
        [Cfg(value = "equal", domain = Constants.ComparisonDomain, toLower = true, ignoreCase = true)]
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

        [Cfg(value = "equal", domain = Constants.ComparisonDomain, toLower = true)]
        public string RunOperator
        {
            get { return _runOperator; }
            set
            {
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
        [Cfg(value = "millisecond", domain = "day,date,dayofweek,dayofyear,hour,millisecond,minute,month,second,tick,year,weekofyear", toLower = true)]
        public string TimeComponent { get; set; }
        [Cfg(value = 0)]
        public int TimeOut { get; set; }

        [Cfg(value = Constants.DefaultSetting, domain = Constants.DefaultSetting + "," + Constants.TypeDomain)]
        public string To { get; set; }
        [Cfg(value = "0.0")]
        public string ToLat { get; set; }
        [Cfg(value = "0.0")]
        public string ToLong { get; set; }
        [Cfg(value = 0)]
        public int TotalWidth { get; set; }

        [Cfg(value = Constants.DefaultSetting, domain = Constants.DefaultSetting + "," + TimeZoneIdDomain)]
        public string ToTimeZone { get; set; }
        [Cfg(value = " ")]
        public string TrimChars { get; set; }
        [Cfg(value = Constants.DefaultSetting, domain = Constants.DefaultSetting + "," + Constants.TypeDomain)]
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

        [Cfg]
        public List<Parameter> Parameters { get; set; }
        [Cfg]
        public List<NameReference> Scripts { get; set; }
        [Cfg]
        public List<NameReference> Templates { get; set; }

        [Cfg]
        public List<Field> Fields { get; set; }

        public bool IsShortHand { get; set; }

        public bool IsValidator() {
            return Validators().Contains(Method);
        }

        [Cfg(value = "firstday", domain = "firstday,firstfourdayweek,firstfullweek", toLower = true)]
        public string CalendarWeekRule { get; set; }

        [Cfg(domain = DayOfWeekDomain, toLower = true)]
        public string DayOfWeek { get; set; }

        [Cfg]
        public DateTime Date { get; set; }

        [Cfg]
        public string Property { get; set; }

        public Func<IRow, bool> ShouldRun { get; set; }

        [Cfg(value=true)]
        public bool Extension { get; set; }

        [Cfg(value="")]
        public string NameSpace { get; set; }

        [Cfg(value="")]
        public string XPath { get; set; }

        public static HashSet<string> Transforms() {
            return _transformSet ?? (_transformSet = new HashSet<string>(TransformerDomain.Split(new[] { ',' })));
        }

        public static HashSet<string> TransformProducers() {
            return _transformProducerSet ?? (_transformProducerSet = new HashSet<string>(TransformProducerDomain.Split(new[] { ',' })));
        }

        public static HashSet<string> Validators() {
            return _validateSet ?? (_validateSet = new HashSet<string>(ValidatorDomain.Split(new[] { ',' })));
        }

        public static HashSet<string> Producers() {
            return _producerSet ?? (_producerSet = new HashSet<string>(ProducerDomain.Split(new[] { ',' })));
        }

        public override string ToString() {
            return Method;
        }
    }
}