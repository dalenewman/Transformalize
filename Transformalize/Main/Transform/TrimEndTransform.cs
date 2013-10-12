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
    public class TrimEndTransform : AbstractTransform
    {
        private readonly char[] _trimCharArray;
        private readonly string _trimChars;

        public TrimEndTransform(string trimChars, IParameters parameters) : base(parameters)
        {
            Name = "Trim End";
            _trimChars = trimChars;
            _trimCharArray = trimChars.ToCharArray();
        }

        public override void Transform(ref StringBuilder sb)
        {
            sb.TrimEnd(_trimChars);
        }

        public override object Transform(object value, string simpleType)
        {
            return value.ToString().TrimEnd(_trimCharArray);
        }

        public override void Transform(ref Row row, string resultKey)
        {
            row[resultKey] = row[FirstParameter.Key].ToString().TrimEnd();
        }
    }
}