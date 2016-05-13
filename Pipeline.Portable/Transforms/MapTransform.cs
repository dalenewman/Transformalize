#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class MapTransform : BaseTransform, ITransform {

        readonly Field _input;
        readonly Dictionary<object, Func<IRow, object>> _map = new Dictionary<object, Func<IRow, object>>();
        private readonly object _catchAll;
        const string CatchAll = "*";

        public MapTransform(PipelineContext context, IMapReader mapReader) : base(context) {
            _input = SingleInput();

            // seems like i have over-complicated this...
            foreach (var item in mapReader.Read(context)) {
                if (item.From.Equals(CatchAll)) {
                    _catchAll = context.Field.Convert(item.To);
                    continue;
                }
                var from = _input.Convert(item.From);
                if (item.To == null || item.To.Equals(string.Empty)) {
                    var field = context.Entity.GetField(item.Parameter);
                    _map[from] = (r) => r[field];
                } else {
                    var to = context.Field.Convert(item.To);
                    _map[from] = (r) => to;
                }
            }
            if (_catchAll == null) {
                _catchAll = context.Field.Convert(context.Field.Default);
            }
        }
        public IRow Transform(IRow row) {

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
