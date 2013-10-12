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
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main
{
    public class SubstringTransform : AbstractTransform
    {
        private readonly int _length;
        private readonly int _startIndex;

        public SubstringTransform(int startIndex, int length, IParameters parameters)
            : base(parameters)
        {
            Name = "Substring";
            _startIndex = startIndex;
            _length = length;
        }

        public override void Transform(ref StringBuilder sb)
        {
            sb.Substring(_startIndex, _length);
        }

        public override object Transform(object value, string simpleType)
        {
            return value.ToString().Substring(_startIndex, _length);
        }

        public override void Transform(ref Row row, string resultKey)
        {
            row[resultKey] = row[FirstParameter.Key].ToString().Substring(_startIndex, _length);
        }
    }
}