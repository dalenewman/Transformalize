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
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.fastJSON;

namespace Transformalize.Main {

    public class FromJsonTransform : AbstractTransform {

        private readonly string _alias;
        private readonly JSON _json = JSON.Instance;

        public FromJsonTransform(string alias, IParameters parameters)
            : base(parameters) {
            _alias = alias;
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

        public override void Transform(ref Row row, string resultKey) {
            var dict = _json.ToObject<Dictionary<string, object>>(row[_alias].ToString());

            foreach (var pair in Parameters) {
                row[pair.Key] = Common.ObjectConversionMap[pair.Value.SimpleType](dict[pair.Value.Name]);
            }
        }
    }
}