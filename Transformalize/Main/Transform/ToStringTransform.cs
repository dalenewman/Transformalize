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
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main {

    public class ToStringTransform : AbstractTransform {
        private readonly string _format;
        private readonly Dictionary<string, Func<object, string, string>> _toString = new Dictionary<string, Func<object, string, string>>() {
            { "datetime", ((value,format) => (Convert.ToDateTime(value)).ToString(format))},
            { "int32", ((value,format) => (Convert.ToInt32(value)).ToString(format))},
            { "decimal", ((value,format) => (Convert.ToDecimal(value)).ToString(format))},
            { "double",  ((value,format) => (Convert.ToDouble(value)).ToString(format))},
            { "int16",  ((value,format) => (Convert.ToInt16(value)).ToString(format))},
            { "int64",  ((value,format) => (Convert.ToInt64(value)).ToString(format))},
            { "byte",  ((value,format) => (Convert.ToByte(value)).ToString(format))},
            { "float",  ((value,format) => ((float)value).ToString(format))},
            { "single",  ((value,format) => (Convert.ToSingle(value)).ToString(format))},
        };

        public ToStringTransform(string format, IParameters parameters)
            : base(parameters) {
            _format = format;
            Name = "ToString (Format)";
        }

        public override void Transform(ref System.Text.StringBuilder sb) {
            Log.Error("You can't use ToString transform on a string.");
            Environment.Exit(1);
        }

        public override object Transform(object value, string simpleType) {
            return _toString[simpleType](value, _format);
        }

        public override void Transform(ref Row row, string resultKey) {
            row[resultKey] = _toString[FirstParameter.Value.SimpleType](row[FirstParameter.Key], _format);
        }
    }
}