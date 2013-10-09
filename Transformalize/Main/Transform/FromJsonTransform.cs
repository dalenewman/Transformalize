#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.fastJSON;

namespace Transformalize.Main {

    public class FromJsonTransform : AbstractTransform {

        private readonly string _alias;
        private readonly bool _clean;
        private readonly JSON _json = JSON.Instance;
        private readonly Regex _start = new Regex(@"^\{{1}""{1}", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _end = new Regex(@"""{1}\}{1}$", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _colon = new Regex(@"""{1}[: ]+""{1}", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _comma = new Regex(@"""{1}[, ]+""{1}", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _fix = new Regex(@"\\?""{1}", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _startBack = new Regex(@"^_SS_", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex _endBack = new Regex(@"_EE_$", RegexOptions.Compiled | RegexOptions.Singleline);

        public FromJsonTransform(string alias, bool clean, IParameters parameters)
            : base(parameters) {
            _alias = alias;
            _clean = clean;
            Name = "From JSON";
        }

        public override void Transform(ref StringBuilder sb) {
            var dict = _json.ToObject<Dictionary<string, object>>(sb.ToString());
            sb.Clear();
            sb.Append(dict[FirstParameter.Value.Name]);
        }

        public override object Transform(object value) {
            var dict = _json.ToObject<Dictionary<string, object>>(value.ToString());
            return Common.ObjectConversionMap[FirstParameter.Value.SimpleType](dict[FirstParameter.Value.Name]);
        }

        public override void Transform(ref Row row, string resultKey)
        {
            var input = _clean ? Clean(row[_alias]) : row[_alias].ToString();
            var dict = _json.ToObject<Dictionary<string, object>>(input);

            foreach (var pair in Parameters) {
                row[pair.Key] = Common.ObjectConversionMap[pair.Value.SimpleType](dict[pair.Value.Name]);
            }
        }

        /// <summary>
        /// An attempt to fix unescaped double quotes within the property or value in a single line of JSON
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string Clean(object input) {
            var output = _start.Replace(input.ToString(), "_SS_");  //tag start with valid quote
            output = _end.Replace(output, "_EE_"); // tag end with valid quote
            output = _colon.Replace(output, "_::_"); // tag colon with valid quotes
            output = _comma.Replace(output, "_,,_"); // tag commas with valid quotes
            output = _fix.Replace(output, @"\"""); // escape the quotes that are left, and re-escape ones that are escaped
            output = _startBack.Replace(output, @"{"""); // put start back
            output = _endBack.Replace(output, @"""}"); // put end back
            output = output.Replace("_::_", @""":"""); // put valid colons back
            return output.Replace("_,,_", @""","""); // put valid commas back
        }

    }
}