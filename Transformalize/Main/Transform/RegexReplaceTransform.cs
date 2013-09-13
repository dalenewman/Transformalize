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
using System.Text.RegularExpressions;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main
{
    public class RegexReplaceTransform : AbstractTransform
    {
        private readonly int _count;
        private readonly Regex _regex;
        private readonly string _replacement;

        public RegexReplaceTransform(string pattern, string replacement, int count, IParameters parameters)
            : base(parameters)
        {
            Name = "Regex Replace";
            _replacement = replacement;
            _count = count;
            _regex = new Regex(pattern, RegexOptions.Compiled);
        }

        public override void Transform(ref StringBuilder sb)
        {
            var input = sb.ToString();
            sb.Clear();
            if (_count > 0)
                sb.Append(_regex.Replace(input, _replacement, _count));
            else
                sb.Append(_regex.Replace(input, _replacement));
        }

        public override object Transform(object value)
        {
            if (_count > 0)
                return _regex.Replace(value.ToString(), _replacement, _count);
            return _regex.Replace(value.ToString(), _replacement);
        }

        public override void Transform(ref Row row, string resultKey)
        {
            if (_count > 0)
            {
                row[resultKey] = _regex.Replace(row[FirstParameter.Key].ToString(), _replacement, _count);
            }
            else
            {
                row[resultKey] = _regex.Replace(row[FirstParameter.Key].ToString(), _replacement);
            }
        }
    }
}