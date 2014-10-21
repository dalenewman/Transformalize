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
using Transformalize.Logging;

namespace Transformalize.Main.Parameters {

    public class Parameter : IParameter {

        private readonly Dictionary<string, Func<object, object>> _conversionMap = Common.GetObjectConversionMap();
        private string _simpleType = "string";

        public int Index { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public bool ValueReferencesField { get; set; }

        public string SimpleType {
            get { return _simpleType; }
            set {
                _simpleType = Common.ToSimpleType(value);
                if (Value != null) {
                    if (_conversionMap.ContainsKey(_simpleType)) {
                        Value = _conversionMap[_simpleType](Value);
                    } else {
                        TflLogger.Warn(string.Empty, string.Empty, "Parameter type {0} is not mapped for conversion.", _simpleType);
                    }
                }
            }
        }

        public Parameter() {
        }

        public Parameter(string name, object value) {
            Name = name;
            Value = value;
        }

        public bool HasValue() {
            return Name != null && Value != null;
        }

    }
}