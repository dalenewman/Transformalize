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
using Transformalize.Transforms;

namespace Transformalize.Validators {

    public class MapValidator : BaseValidate {

        private readonly Field _input;
        private readonly HashSet<object> _map = new HashSet<object>();
        private BetterFormat _betterFormat;

        public MapValidator(IContext context) : base(context) {

            if (!Run)
                return;

            if (context.Operation.Map == string.Empty) {
                Error("The map method requires a map");
                Run = false;
                return;
            }

            _input = SingleInput();

        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {

            var map = CreateMap();
            foreach (var item in map.Items) {
                _map.Add(_input.Convert(item.From));
            }

            var help = Context.Field.Help;
            if (help == string.Empty) {
                help = $"{Context.Field.Label}'s value {{{Context.Field.Alias}}} is not found in {_map.Count} items.";
            }
            _betterFormat = new BetterFormat(Context, help, Context.Entity.GetAllFields);

            return base.Operate(rows);
        }

        private Map CreateMap() {

            var map = Context.Process.Maps.FirstOrDefault(m => m.Name == Context.Operation.Map);

            if (map != null)
                return map;

            // auto map
            map = new Map { Name = Context.Operation.Map };
            var split = Context.Operation.Map.Split(',');
            foreach (var item in split) {
                map.Items.Add(new MapItem { From = item, To = item });
            }
            Context.Process.Maps.Add(map);

            return map;
        }

        public override IRow Operate(IRow row) {
            var valid = _map.Contains(row[_input]);
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, _betterFormat.Format(row));
            }
            Increment();
            return row;
        }
    }

}
