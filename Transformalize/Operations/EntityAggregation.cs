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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {

    public class EntityAggregation : AbstractAggregationOperation {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly IDictionary<string, ConcurrentDictionary<ObjectArrayKeys, List<object>>> _lists = new Dictionary<string, ConcurrentDictionary<ObjectArrayKeys, List<object>>>();
        private readonly IDictionary<string, Dictionary<ObjectArrayKeys, Dictionary<object, byte>>> _distinct = new Dictionary<string, Dictionary<ObjectArrayKeys, Dictionary<object, byte>>>();
        private readonly Field[] _fieldsToAccumulate;
        private readonly string[] _keysToGroupBy;
        private readonly string _firstKey;

        public EntityAggregation(Entity entity) {

            _keysToGroupBy = new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Context().ToEnumerable().Where(f => f.Aggregate.Equals("group", IC)).Select(f => f.Alias).ToArray();
            _firstKey = _keysToGroupBy[0];

            _fieldsToAccumulate = new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Context().ToEnumerable().Where(f => !f.Aggregate.Equals("group", IC)).ToArray();

            foreach (var field in _fieldsToAccumulate) {
                _lists[field.Alias] = new ConcurrentDictionary<ObjectArrayKeys, List<object>>();
                if (field.Aggregate.Equals("countdistinct", IC)) {
                    _distinct[field.Alias] = new Dictionary<ObjectArrayKeys, Dictionary<object, byte>>();
                }
            }
        }

        protected override void Accumulate(Row row, Row aggregate) {
            //init
            if (!aggregate.ContainsKey(_firstKey)) {
                foreach (var column in _keysToGroupBy) {
                    aggregate[column] = row[column];
                }

                foreach (var field in _fieldsToAccumulate) {
                    if (field.Aggregate.StartsWith("count", IC)) {
                        aggregate[field.Alias] = 0;
                    } else {
                        aggregate[field.Alias] = field.Default ?? new DefaultFactory().Convert(string.Empty, field.SimpleType);
                    }
                }
            }

            //accumulate
            foreach (var field in _fieldsToAccumulate) {
                ObjectArrayKeys key;
                switch (field.Aggregate) {
                    case "count":
                        aggregate[field.Alias] = (int)aggregate[field.Alias] + 1;
                        break;
                    case "countdistinct":
                        key = row.CreateKey(_keysToGroupBy);
                        var value = row[field.Name];
                        if (!_distinct[field.Alias].ContainsKey(key)) {
                            _distinct[field.Alias].Add(key, new Dictionary<object, byte>());
                        }
                        if (!_distinct[field.Alias][key].ContainsKey(value)) {
                            _distinct[field.Alias][key].Add(value, 0);
                            aggregate[field.Alias] = (int)aggregate[field.Alias] + 1;
                        }
                        break;
                    case "sum":
                        switch (field.SimpleType) {
                            case "int32":
                                aggregate[field.Alias] = (int)aggregate[field.Alias] + (int)row[field.Alias];
                                break;
                        }
                        break;
                    case "max":
                        switch (field.SimpleType) {
                            case "int32":
                                aggregate[field.Alias] = Math.Max((int)aggregate[field.Alias], (int)row[field.Alias]);
                                break;
                            case "int64":
                                aggregate[field.Alias] = Math.Max((long)aggregate[field.Alias], (long)row[field.Alias]);
                                break;
                            case "byte[]":
                                aggregate[field.Alias] = Common.Max((byte[])aggregate[field.Alias], (byte[])row[field.Alias]);
                                break;
                            case "string":
                                aggregate[field.Alias] = (new[] { aggregate[field.Alias].ToString(), row[field.Alias].ToString() }).Max();
                                break;
                            case "guid":
                                aggregate[field.Alias] = (new[] { (Guid)aggregate[field.Alias], (Guid)row[field.Alias] }).Max();
                                break;

                        }
                        break;
                    case "last":
                        aggregate[field.Alias] = row[field.Alias];
                        break;

                    case "join":
                        Keep(field.Alias, row);
                        break;

                    case "array":
                        Keep(field.Alias, row);
                        break;
                }
            }
        }

        protected override string[] GetColumnsToGroupBy() {
            return _keysToGroupBy;
        }

        private void Keep(string alias, Row row) {
            var key = row.CreateKey(_keysToGroupBy);
            if (_lists[alias].ContainsKey(key)) {
                _lists[alias][key].Add(row[alias]);
            } else {
                _lists[alias][key] = new List<object>() { row[alias] };
            }
        }

        protected override void FinishAggregation(Row aggregate) {
            //final accumulate
            var group = aggregate.CreateKey(_keysToGroupBy);
            foreach (var field in _fieldsToAccumulate) {
                switch (field.Aggregate) {
                    case "join":
                        aggregate[field.Alias] = string.Join(field.Delimiter,_lists[field.Alias][group]);
                        break;
                    case "array":
                        aggregate[field.Alias] = _lists[field.Alias][group].ToArray();
                        break;
                }
            }
        }
    }
}