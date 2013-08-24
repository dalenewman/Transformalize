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
using System.Linq;
using System.Text;
using Transformalize.Core.Fields_;
using Transformalize.Core.Parameters_;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_
{

    public class MapTransform : AbstractTransform
    {
        private readonly Map _equals;
        private readonly Map _startsWith;
        private readonly Map _endsWith;

        public MapTransform(IList<Map> maps, IParameters parameters, IFields results) : base(parameters, results)
        {
            _equals = maps[0];
            _startsWith = maps[1];
            _endsWith = maps[2];
        }

        protected override string Name
        {
            get { return "Map Transform"; }
        }

        public override void Transform(ref StringBuilder sb)
        {
            foreach (var pair in _equals)
            {
                if (!sb.IsEqualTo(pair.Key)) continue;
                sb.Clear();
                sb.Append(pair.Value.Value);
                return;
            }

            foreach (var pair in _startsWith)
            {
                if (!sb.StartsWith(pair.Key)) continue;
                sb.Clear();
                sb.Append(pair.Value.Value);
                return;
            }

            foreach (var pair in _endsWith)
            {
                if (!sb.EndsWith(pair.Key)) continue;
                sb.Clear();
                sb.Append(pair.Value.Value);
                return;
            }

            if (!_equals.ContainsKey("*")) return;

            sb.Clear();
            sb.Append(_equals["*"].Value);
        }

        public override void Transform(ref object value)
        {
            var valueKey = value.ToString();

            if (_equals.ContainsKey(valueKey))
            {
                value = _equals[valueKey].Value;
                return;
            }

            foreach (var pair in _startsWith.Where(pair => valueKey.StartsWith(pair.Key)))
            {
                value = pair.Value.Value;
                return;
            }

            foreach (var pair in _endsWith.Where(pair => valueKey.EndsWith(pair.Key)))
            {
                value = pair.Value.Value;
                return;
            }

            if (_equals.ContainsKey("*"))
            {
                value = _equals["*"].Value;
            }

        }

        public override void Transform(ref Row row)
        {
            var valueKey = row[FirstParameter.Key].ToString();
            
            if (_equals.ContainsKey(valueKey))
            {
                row[FirstResult.Key] = _equals[valueKey].Value ?? row[_equals[valueKey].Parameter];
                return;
            }

            foreach (var pair in _startsWith.Where(pair => valueKey.StartsWith(pair.Key)))
            {
                row[FirstResult.Key] = pair.Value.Value ?? row[pair.Value.Parameter];
                return;
            }

            foreach (var pair in _endsWith.Where(pair => valueKey.EndsWith(pair.Key)))
            {
                row[FirstResult.Key] = pair.Value.Value ?? row[pair.Value.Parameter];
                return;
            }

            if (_equals.ContainsKey("*"))
            {
                row[FirstResult.Key] = _equals["*"].Value ?? row[_equals["*"].Parameter];
            }

        }

    }
}