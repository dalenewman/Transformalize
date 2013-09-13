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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main
{
    public class MapTransform : AbstractTransform
    {
        private readonly Map _endsWith;
        private readonly Map _equals;
        private readonly bool _hasEndsWith;
        private readonly bool _hasEquals;
        private readonly bool _hasStartsWith;
        private readonly Map _startsWith;

        public MapTransform(IList<Map> maps, IParameters parameters)
            : base(parameters)
        {
            Name = "Map";
            _equals = maps[0];
            _hasEquals = _equals.Any();
            _startsWith = maps[1];
            _hasStartsWith = _startsWith.Any();
            _endsWith = maps[2];
            _hasEndsWith = _endsWith.Any();
        }

        public override void Transform(ref StringBuilder sb)
        {
            if (_hasEquals)
            {
                foreach (var pair in _equals)
                {
                    if (!sb.IsEqualTo(pair.Key)) continue;
                    sb.Clear();
                    sb.Append(pair.Value.Value);
                    return;
                }
            }

            if (_hasStartsWith)
            {
                foreach (var pair in _startsWith)
                {
                    if (!sb.StartsWith(pair.Key)) continue;
                    sb.Clear();
                    sb.Append(pair.Value.Value);
                    return;
                }
            }

            if (_hasEndsWith)
            {
                foreach (var pair in _endsWith)
                {
                    if (!sb.EndsWith(pair.Key)) continue;
                    sb.Clear();
                    sb.Append(pair.Value.Value);
                    return;
                }
            }

            if (!_equals.ContainsKey("*")) return;

            sb.Clear();
            sb.Append(_equals["*"].Value);
        }

        public override object Transform(object value)
        {
            var valueKey = value.ToString();

            if (_hasEquals)
            {
                if (_equals.ContainsKey(valueKey))
                {
                    return _equals[valueKey].Value;
                }
            }

            if (_hasStartsWith)
            {
                foreach (var pair in _startsWith.Where(pair => valueKey.StartsWith(pair.Key)))
                {
                    return pair.Value.Value;
                }
            }

            if (_hasEndsWith)
            {
                foreach (var pair in _endsWith.Where(pair => valueKey.EndsWith(pair.Key)))
                {
                    return pair.Value.Value;
                }
            }

            if (_equals.ContainsKey("*"))
            {
                return _equals["*"].Value;
            }

            return null;
        }

        public override void Transform(ref Row row, string resultKey)
        {
            var valueKey = row[FirstParameter.Key].ToString();

            if (_hasEquals)
            {
                if (_equals.ContainsKey(valueKey))
                {
                    row[resultKey] = _equals[valueKey].Value ?? row[_equals[valueKey].Parameter];
                    return;
                }
            }

            if (_hasStartsWith)
            {
                foreach (var pair in _startsWith.Where(pair => valueKey.StartsWith(pair.Key)))
                {
                    row[resultKey] = pair.Value.Value ?? row[pair.Value.Parameter];
                    return;
                }
            }

            if (_hasEndsWith)
            {
                foreach (var pair in _endsWith.Where(pair => valueKey.EndsWith(pair.Key)))
                {
                    row[resultKey] = pair.Value.Value ?? row[pair.Value.Parameter];
                    return;
                }
            }

            if (_equals.ContainsKey("*"))
            {
                row[resultKey] = _equals["*"].Value ?? row[_equals["*"].Parameter];
            }
        }
    }
}