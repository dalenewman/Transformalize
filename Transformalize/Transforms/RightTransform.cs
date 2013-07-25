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
using System.Text;
using Transformalize.Extensions;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class RightTransform : AbstractTransform {
        private readonly int _length;

        public RightTransform(int length) {
            _length = length;
        }

        protected override string Name {
            get { return "Right Transform"; }
        }

        public override void Transform(ref StringBuilder sb) {
            sb.Right(_length);
        }

        public override void Transform(ref object value) {
            value = value.ToString().Right(_length);
        }
    }
}