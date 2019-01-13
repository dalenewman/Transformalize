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
using System.Collections.Generic;
using System.Linq;

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

        [Cfg]
        public string Query { get; set; }

        [Cfg]
        public string Command { get; set; }

        public Configuration.Operation ToOperation() {
            var operation = new Configuration.Operation {
                Name = this.Name,
                AdministrativeArea = this.AdministrativeArea,
                Body = this.Body,
                CalendarWeekRule = this.CalendarWeekRule,
                Class = this.Class,
                Connection = this.Connection,
                ContentType = this.ContentType,
                Country = this.Country,
                DayOfWeek = this.DayOfWeek,
                Direction = this.Direction,
                Domain = this.Domain,
                Expression = this.Expression,
                FalseField = this.FalseField,
                Format = this.Format,
                Fields = new List<Configuration.Field>(),
                FromLat = this.FromLat,
                FromLon = this.FromLon,
                FromTimeZone = this.FromTimeZone,
                HRef = this.HRef,
                Latitude = this.Latitude,
                Level = this.Level,
                Locality = this.Locality,
                Longitude = this.Longitude,
                Method = this.Method,
                Map = this.Map,
                Mode = this.Mode,
                NewValue = this.NewValue,
                OldValue = this.OldValue,
                Operator = this.Operator,
                PaddingChar = this.PaddingChar,
                Parameter = this.Parameter,
                Pattern = this.Pattern,
                PostalCode = this.PostalCode,
                Role = this.Role,
                Root = this.Root,
                Route = this.Route,
                RunField = this.RunField,
                RunOperator = this.RunOperator,
                RunValue = this.RunValue,
                Script = this.Script,
                Src = this.Src,
                Tag = this.Tag,
                Target = this.Target,
                Template = this.Template,
                TimeComponent = this.TimeComponent,
                Title = this.Title,
                ToLat = this.ToLat,
                ToLon = this.ToLon,
                ToTimeZone = this.ToTimeZone,
                Separator = this.Separator,
                TrimChars = this.TrimChars,
                TrueField = this.TrueField,
                Type = this.Type,
                Units = this.Units,
                Url = this.Url,
                Value = this.Value,
                WebMethod = this.WebMethod,
                XmlMode = this.XmlMode,
                Query = this.Query,
                Command = this.Command
            };

            int.TryParse(this.Count, out var count);
            operation.Count = count;

            int.TryParse(this.Decimals, out var decimals);
            operation.Decimals = decimals;

            bool.TryParse(this.Encode, out var encode);
            operation.Encode = encode;

            bool.TryParse(this.Extension, out var extension);
            operation.Extension = extension;

            int.TryParse(this.Height, out var height);
            operation.Height = height;

            int.TryParse(this.Index, out var index);
            operation.Index = index;

            int.TryParse(this.Length, out var length);
            operation.Length = length;

            int.TryParse(this.Limit, out var limit);
            operation.Limit = limit;

            operation.Parameters = this.Parameters.Select(p => p.ToParameter()).ToList();

            operation.Scripts = this.Scripts.Select(s => s.ToNameReference()).ToList();

            int.TryParse(this.Seed, out var seed);
            operation.Seed = seed;

            int.TryParse(this.StartIndex, out var startIndex);
            operation.StartIndex = startIndex;

            int.TryParse(this.Time, out var time);
            operation.Time = time;

            int.TryParse(this.TotalWidth, out var totalWidth);
            operation.TotalWidth = totalWidth;

            int.TryParse(this.Width, out var width);
            operation.Width = width;

            return operation;
        }

    }
}