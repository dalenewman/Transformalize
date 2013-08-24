using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityAggregation : AbstractAggregationOperation {
        private readonly Entity _entity;
        private readonly char _separator;
        private readonly char[] _separatorArray;
        private readonly string _separatorString;
        private readonly string[] _columnsToGroupBy;
        private readonly Field[] _columnsToAccumulate;
        private readonly string _firstKey;
        private readonly IDictionary<string, StringBuilder> _builders = new Dictionary<string, StringBuilder>();

        public EntityAggregation(Entity entity, char separator = ',') {
            _entity = entity;
            _separator = separator;
            _separatorString = separator.ToString(CultureInfo.InvariantCulture);
            _separatorArray = new[] { separator };
            _columnsToGroupBy = new FieldSqlWriter(_entity.PrimaryKey).ExpandXml().Input().Context().ToEnumerable().Select(f=>f.Alias).ToArray();
            _firstKey = _columnsToGroupBy[0];
            _columnsToAccumulate = new FieldSqlWriter(_entity.All).ExpandXml().Input().Aggregate().ToArray();

            foreach (var field in _columnsToAccumulate) {
                _builders[field.Alias] = new StringBuilder();
            }
        }

        protected override void Accumulate(Row row, Row aggregate) {
            //init
            if (!aggregate.ContainsKey(_firstKey)) {
                foreach (var column in _columnsToGroupBy) {
                    aggregate[column] = row[column];
                }

                foreach (var field in _columnsToAccumulate) {
                    aggregate[field.Alias] = field.Default ?? new ConversionFactory().Convert(string.Empty, field.SimpleType);
                }
            }

            //accumulate
            foreach (var field in _columnsToAccumulate) {
                switch (field.Aggregate) {
                    case "sum":
                        switch (field.SimpleType) {
                            case "int32":
                                aggregate[field.Alias] = (int)aggregate[field.Alias] + (int)row[field.Alias];
                                break;
                            default:
                                break;
                        }
                        break;
                    case "max":
                        switch (field.SimpleType)
                        {
                            case "int32":
                                aggregate[field.Alias] = Math.Max((int)aggregate[field.Alias],(int)row[field.Alias]);
                                break;
                            case "int64":
                                aggregate[field.Alias] = Math.Max((long)aggregate[field.Alias], (long)row[field.Alias]);
                                break;
                            default:
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
            return _columnsToGroupBy;
        }

        protected override void FinishAggregation(Row aggregate) {
            //final accumulate
            foreach (var field in _columnsToAccumulate) {
                switch (field.Aggregate) {
                    case "join":
                        var aggregateValue = aggregate[field.Alias].ToString();
                        var aggregateIsEmpty = aggregateValue == string.Empty;

                        if (aggregateIsEmpty)
                            break;

                        aggregate[field.Alias] = string.Join(_separatorString + " ",aggregateValue.Split(_separatorArray).Select(s=>s.Trim()).Distinct());
                        break;
                    default:
                        break;
                }
            }

        }
    }
}