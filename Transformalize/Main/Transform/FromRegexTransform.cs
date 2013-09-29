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

namespace Transformalize.Main {

    public class FromRegexTransform : AbstractTransform {

        private readonly Dictionary<string, string> _map = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _typeMap = new Dictionary<string, string>();
        private readonly string _alias;
        private readonly Regex _regex;

        public FromRegexTransform(string alias, string pattern, IParameters parameters)
            : base(parameters) {
            _alias = alias;
            Name = "From Regex";
            _regex = new Regex(pattern, RegexOptions.Compiled);

            foreach (var field in Parameters) {
                _map[field.Value.Name] = field.Key;
                // in case of XML, the key should be the field's new alias (if present)
            }

            foreach (var field in Parameters) {
                _typeMap[field.Value.Name] = field.Value.SimpleType;
                // in case of XML, the name is the name of the XML element or attribute
            }
        }

        public override void Transform(ref StringBuilder sb) {
            var input = sb.ToString();
            var match = _regex.Match(input);
            if (!match.Success) {
                return;
            }
            sb.Clear();
            sb.Append(match.Value);
        }

        public override object Transform(object value) {
            var input = value.ToString();
            var match = _regex.Match(input);
            return match.Success ? match.Value : value;
        }

        public override void Transform(ref Row row, string resultKey) {
            var match = _regex.Match(row[_alias].ToString());

            if (match.Groups.Count == 0)
                return;

            foreach (var pair in Parameters) {
                var group = match.Groups[pair.Key];
                if (@group != null) {
                    row[pair.Key] = Common.ConversionMap[pair.Value.SimpleType](@group.Value);
                }
            }
        }
    }
}