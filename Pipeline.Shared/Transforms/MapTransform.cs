#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class MapTransform : BaseTransform {

        readonly Field _input;
        readonly Dictionary<object, Func<IRow, object>> _map = new Dictionary<object, Func<IRow, object>>();
        private object _catchAll;
        const string CatchAll = "*";

        public MapTransform(IContext context) : base(context, null) {
            _input = SingleInput();
        }

        public override IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {

            var map = Context.Process.Maps.First(m => m.Name == Context.Transform.Map);

            // seems like i have over-complicated this...
            foreach (var item in map.Items) {
                if (item.From.Equals(CatchAll)) {
                    _catchAll = Context.Field.Convert(item.To);
                    continue;
                }
                var from = _input.Convert(item.From);
                if (item.To == null || item.To.Equals(string.Empty)) {
                    var field = Context.Entity.GetField(item.Parameter);
                    _map[from] = (r) => r[field];
                } else {
                    var to = Context.Field.Convert(item.To);
                    _map[from] = (r) => to;
                }
            }
            if (_catchAll == null) {
                _catchAll = Context.Field.Convert(Context.Field.Default);
            }

            return base.Transform(rows);
        }

        public override IRow Transform(IRow row) {

            Func<IRow, object> objects;
            if (_map.TryGetValue(row[_input], out objects)) {
                row[Context.Field] = objects(row);
            } else {
                row[Context.Field] = _catchAll;
            }
            Increment();
            return row;
        }
    }

}
