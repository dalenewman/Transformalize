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

namespace Transformalize.Transforms {
    public class PadLeftTransform : Transformer {
        private readonly int _totalWidth;
        private readonly char _paddingChar;

        public PadLeftTransform(int totalWidth, char paddingChar) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
        }

        public PadLeftTransform(int totalWidth, char paddingChar, Dictionary<string, Field> parameters, Dictionary<string, Field> results)
            : base(parameters, results) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
        }

        protected override string Name {
            get { return "PadLeft Transform"; }
        }

        public override void Transform(ref StringBuilder sb) {
            sb.PadLeft(_totalWidth, _paddingChar);
        }

        public override void Transform(ref object value) {
            value = value.ToString().PadLeft(_totalWidth, _paddingChar);
        }

    }
}