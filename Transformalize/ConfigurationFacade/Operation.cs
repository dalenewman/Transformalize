#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Transformalize.Contracts;

namespace Transformalize.ConfigurationFacade {

    public class Operation : CfgNode {

        [Cfg]
        public string Connection { get; set; }

        [Cfg]
        public string ContentType { get; set; }

        [Cfg]
        public string Count { get; set; }

        [Cfg]
        public string Domain { get; set; }

        [Cfg]
        public string Encode { get; set; }

        [Cfg]
        public string Format { get; set; }

        [Cfg]
        public string FromLat { get; set; }

        [Cfg]
        public string FromLon { get; set; }

        [Cfg]
        public string FromTimeZone { get; set; }

        [Cfg]
        public string Index { get; set; }

        [Cfg]
        public string Length { get; set; }

        [Cfg]
        public string Map { get; set; }

        [Cfg]
        public string Method { get; set; }

        [Cfg]
        public string Name { get; set; }

        [Cfg]
        public string Negated { get; set; }

        [Cfg]
        public string NewValue { get; set; }

        [Cfg]
        public string OldValue { get; set; }

        [Cfg]
        public string Operator { get; set; }

        [Cfg]
        public char PaddingChar { get; set; }

        [Cfg]
        public string Parameter { get; set; }

        [Cfg]
        public string Pattern { get; set; }

        [Cfg]
        public string Root { get; set; }

        [Cfg]
        public string RunField { get; set; }

        [Cfg]
        public string RunOperator { get; set; }
        
        [Cfg]
        public string RunValue { get; set; }

        [Cfg]
        public string Script { get; set; }

        [Cfg]
        public string Separator { get; set; }

        [Cfg]
        public string StartIndex { get; set; }

        [Cfg]
        public string Tag { get; set; }

        [Cfg]
        public string HRef { get; set; }

        [Cfg]
        public string Class { get; set; }

        [Cfg]
        public string Title { get; set; }

        [Cfg]
        public string Style { get; set; }

        [Cfg]
        public string Role { get; set; }

        [Cfg]
        public string Target { get; set; }

        [Cfg]
        public string Body { get; set; }

        [Cfg]
        public string Src { get; set; }

        [Cfg]
        public string Width { get; set; }

        [Cfg]
        public string Height { get; set; }

        [Cfg]
        public string Template { get; set; }

        [Cfg]
        public string TimeComponent { get; set; }

        [Cfg]
        public string ToLat { get; set; }

        [Cfg]
        public string ToLon { get; set; }

        [Cfg]
        public string TotalWidth { get; set; }

        [Cfg]
        public string ToTimeZone { get; set; }

        [Cfg]
        public string TrimChars { get; set; }

        [Cfg]
        public string Type { get; set; }

        [Cfg]
        public string Units { get; set; }

        [Cfg]
        public string Url { get; set; }

        [Cfg]
        public string Value { get; set; }

        [Cfg]
        public string WebMethod { get; set; }

        [Cfg]
        public string Mode { get; set; }

        [Cfg]
        public string XmlMode { get; set; }

        [Cfg]
        public List<Parameter> Parameters { get; set; }

        [Cfg]
        public List<NameReference> Scripts { get; set; }

        [Cfg]
        public List<Field> Fields { get; set; }

        [Cfg]
        public string CalendarWeekRule { get; set; }

        [Cfg]
        public string DayOfWeek { get; set; }

        [Cfg]
        public string Property { get; set; }

        [Cfg]
        public string Extension { get; set; }

        [Cfg]
        public string NameSpace { get; set; }

        [Cfg]
        public string Decimals { get; set; }

        [Cfg]
        public string Expression { get; set; }

        [Cfg]
        public string TrueField { get; set; }

        [Cfg]
        public string FalseField { get; set; }

        [Cfg]
        public string Latitude { get; set; }

        [Cfg]
        public string Longitude { get; set; }

        [Cfg]
        public string Direction { get; set; }

        [Cfg]
        public string Key { get; set; }

        [Cfg]
        public string Limit { get; set; }

        [Cfg]
        public string AdministrativeArea { get; set; }

        [Cfg]
        public string Route { get; set; }

        [Cfg]
        public string Country { get; set; }

        [Cfg]
        public string Locality { get; set; }

        [Cfg]
        public string PostalCode { get; set; }

        [Cfg]
        public string Time { get; set; }

        [Cfg]
        public string Level { get; set; }

        [Cfg]
        public string Seed { get; set; }

        [Cfg]
        public string Step { get; set; }

    }
}