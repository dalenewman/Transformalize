/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class FormatTransform : AbstractTransform {
        private readonly string _format;
        private int _index;

        protected override string Name {
            get { return "Format Transform"; }
        }

        public FormatTransform(string format, IParameters parameters, Dictionary<string, Field> results)
            : base(parameters, results) {
            _format = format;
        }

        public override void Transform(ref StringBuilder sb) {
            var value = sb.ToString();
            sb.Clear();
            sb.AppendFormat(_format, value);
        }

        public override void Transform(ref object value) {
            value = string.Format(_format, value);
        }

        public override void Transform(ref Row row) {
            _index = 0;
            foreach (var pair in Parameters) {
                ParameterValues[_index] = pair.Value.Value ?? row[pair.Key];
                _index++;
            }

            row[FirstResult.Key] = string.Format(_format, ParameterValues);
        }

    }
}