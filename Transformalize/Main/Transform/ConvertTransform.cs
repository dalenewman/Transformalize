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

using System;
using System.Collections.Generic;
using System.Text;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main {

    public class ConvertTransform : AbstractTransform {
        private readonly string _format;
        private readonly Dictionary<string, Func<object, object>> _conversionMap = new Dictionary<string, Func<object, object>>();
        private readonly Dictionary<string, Func<object, object>> _specialConversionMap = new Dictionary<string, Func<object, object>>();
        private readonly string _to;

        public ConvertTransform(string to, string format, IParameters parameters)
            : base(parameters) {
            _format = format;
            Name = "Convert";
            _to = Common.ToSimpleType(to);

            if (HasParameters && FirstParameter.Value.SimpleType == "datetime" && _to == "int32") {
                _specialConversionMap.Add("int32", (x => Common.DateTimeToInt32((DateTime)x)));
                _conversionMap = _specialConversionMap;
            } else {
                if (_to == "datetime" && !string.IsNullOrEmpty(_format)) {
                    _specialConversionMap.Add("datetime", (x => DateTime.ParseExact(x.ToString(), _format, System.Globalization.CultureInfo.InvariantCulture)));
                    _conversionMap = _specialConversionMap;
                } else {
                    _conversionMap = Common.ObjectConversionMap;
                }
            }

        }

        public override void Transform(ref StringBuilder sb) {
            var input = sb.ToString();
            sb.Clear();
            sb.Append(_conversionMap[_to](input));
        }

        public override object Transform(object value) {
            return _conversionMap[_to](value);
        }

        public override void Transform(ref Row row, string resultKey) {
            row[resultKey] = _conversionMap[_to](row[FirstParameter.Key]);
        }
    }
}