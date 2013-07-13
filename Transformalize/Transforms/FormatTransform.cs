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
    public class FormatTransform : ITransform {
        private readonly string _format;
        public Dictionary<string, Field> Parameters { get; private set; }
        public Dictionary<string, Field> Results { get; private set; } 
        private readonly bool _hasParameters;
        private readonly bool _hasResults;
        private readonly object[] _parameterValues;
        private int _index;

        public bool HasParameters {
            get { return _hasParameters; }
        }
        public bool HasResults {
            get { return _hasResults; }
        }

        public FormatTransform(string format, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _format = format;
            Parameters = parameters;
            Results = results;
            _hasParameters = parameters != null && parameters.Count > 0;
            _hasResults = results != null && results.Count > 0;

            if (!_hasParameters) return;

            _parameterValues = new object[Parameters.Count];
        }

        public FormatTransform(string format) : this(format, null, null) { }

        public void Transform(ref StringBuilder sb) {
            var value = sb.ToString();
            sb.Clear();
            sb.AppendFormat(_format, value);
        }

        public void Transform(ref object value) {
            value = string.Format(_format, value);
        }

        public void Transform(ref Row row) {
            _index = 0;
            foreach (var pair in Parameters) {
                _parameterValues[_index] = row[pair.Key];
                _index++;
            }

            var result = string.Format(_format, _parameterValues);
            foreach (var pair in Results) {
                row[pair.Key] = result;
            }
        }

        public void Dispose() {
        }

    }
}