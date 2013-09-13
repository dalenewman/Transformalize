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

namespace Transformalize.Main
{
    public class JoinTransform : AbstractTransform
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly int _count;
        private readonly string _separator;
        private int _index;

        public JoinTransform(string separator, IParameters parameters)
            : base(parameters)
        {
            _separator = separator;
            _count = Parameters.Count;
            RequiresParameters = true;
            Name = "Join";
        }

        public override void Transform(ref Row row, string resultKey)
        {
            _index = 0;
            _builder.Clear();
            foreach (var pair in Parameters)
            {
                _builder.Append(pair.Value.Value ?? row[pair.Key]);
                _index++;
                if (_index < _count)
                    _builder.Append(_separator);
            }
            row[resultKey] = _builder.ToString();
        }
    }
}