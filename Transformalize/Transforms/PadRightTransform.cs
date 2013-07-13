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
    public class PadRightTransform : ITransform {
        private readonly int _totalWidth;
        private readonly char _paddingChar;
        public Dictionary<string, Field> Parameters { get; private set; }
        public Dictionary<string, Field> Results { get; private set; }

        public PadRightTransform(int totalWidth, char paddingChar) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
        }

        public PadRightTransform(int totalWidth, char paddingChar, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
            Parameters = parameters;
            Results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            sb.PadRight(_totalWidth, _paddingChar);
        }

        public void Transform(ref object value) {
            value = value.ToString().PadRight(_totalWidth, _paddingChar);
        }

        public void Transform(ref Row row) {

        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }


    }
}