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
using Transformalize.Core.Fields_;
using Transformalize.Core.Parameters_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.fastJSON;

namespace Transformalize.Core.Transform_ {
    public class JsonTransform : AbstractTransform {
        protected override string Name { get { return "Json Transform"; } }
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public JsonTransform(IParameters parameters, IFields results)
            : base(parameters, results) {
        }

        public override void Transform(ref Row row) {
            _values.Clear();
            foreach (var pair in Parameters) {
                _values[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
            }

            row[FirstResult.Key] = JSON.Instance.ToJSON(_values);
        }

    }
}