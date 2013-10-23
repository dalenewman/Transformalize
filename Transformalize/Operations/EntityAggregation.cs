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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class EntityAggregation : AbstractAggregationOperation {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly IDictionary<string, StringBuilder> _builders = new Dictionary<string, StringBuilder>();
        private readonly Field[] _fieldsToAccumulate;
        private readonly string[] _keysToGroupBy;
        private readonly string _firstKey;
        private readonly char _separator;
        private readonly char[] _separatorArray;
        private readonly string _separatorString;

        public EntityAggregation(Entity entity, char separator = ',') {
            _separator = separator;
            _separatorString = separator.ToString(CultureInfo.InvariantCulture);
            _separatorArray = new[] { separator };

            _keysToGroupBy = new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Context().ToEnumerable().Where(f=> f.Output && f.Aggregate.Equals("group", IC)).Select(f=>f.Alias).ToArray();
            _firstKey = _keysToGroupBy[0];
            
            _fieldsToAccumulate = new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Context().ToEnumerable().Where(f => f.Output && !f.Aggregate.Equals("group", IC)).ToArray();
            

            foreach (var field in _fieldsToAccumulate) {
                _builders[field.Alias] = new StringBuilder();
            }
        }

        protected override void Accumulate(Row row, Row aggregate) {
            //init
            if (!aggregate.ContainsKey(_firstKey)) {
                foreach (var column in _keysToGroupBy) {
                    aggregate[column] = row[column];
                }

                foreach (var field in _fieldsToAccumulate) {
                    aggregate[field.Alias] = field.Default ?? new DefaultFactory().Convert(string.Empty, field.SimpleType);
                }
            }

            //accumulate
            foreach (var field in _fieldsToAccumulate) {
                switch (field.Aggregate) {
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
                        }
                        break;
                    case "join":
                        var aggregateValue = aggregate[field.Alias].ToString();
                        var aggregateIsEmpty = aggregateValue == string.Empty;

                        var rowValue = row[field.Alias].ToString().Replace(_separatorString, string.Empty);
                        var rowIsEmpty = rowValue == string.Empty;

                        if (aggregateIsEmpty && rowIsEmpty)
                            break;

                        if (!aggregateIsEmpty) {
                            _builders[field.Alias].Clear();
                            _builders[field.Alias].Append(aggregateValue);
                            if (!rowIsEmpty && aggregateValue != rowValue) {
                                _builders[field.Alias].Append(_separator);
                                _builders[field.Alias].Append(" ");
                                _builders[field.Alias].Append(rowValue);
                            }
                            aggregate[field.Alias] = _builders[field.Alias].ToString();
                        } else {
                            aggregate[field.Alias] = rowValue;
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        protected override string[] GetColumnsToGroupBy() {
            return _keysToGroupBy;
        }

        protected override void FinishAggregation(Row aggregate) {
            //final accumulate
            foreach (var field in _fieldsToAccumulate) {
                switch (field.Aggregate) {
                    case "join":
                        var aggregateValue = aggregate[field.Alias].ToString();
                        var aggregateIsEmpty = aggregateValue == string.Empty;

                        if (aggregateIsEmpty)
                            break;

                        aggregate[field.Alias] = string.Join(_separatorString + " ", aggregateValue.Split(_separatorArray).Select(s => s.Trim()).Distinct());
                        break;
                    default:
                        break;
                }
            }
        }
    }
}