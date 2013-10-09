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

using System.Text;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main {

    public class DefaultEmptyTransform : AbstractTransform {
        private readonly Process _process;
        private readonly string _alias;
        private static Field _field;

        private Field Field {
            get {
                return _field ?? (_field = _process.GetField(_alias));
            }
        }

        public DefaultEmptyTransform(Process process, string alias, IParameters parameters)
            : base(parameters) {
            _process = process;
            _alias = alias;
            Name = "Replace";
        }

        public override void Transform(ref StringBuilder sb) {

            if (sb.Length == 0) {
                sb.Append(Field.Default);
            }
        }

        public override object Transform(object value) {
            return value.Equals(string.Empty) ? Field.Default : value;
        }

        public override void Transform(ref Row row, string resultKey) {
            if (row[FirstParameter.Key] == null || row[FirstParameter.Key].ToString() == string.Empty) {
                row[FirstParameter.Key] = Field.Default;
            }
        }
    }
}