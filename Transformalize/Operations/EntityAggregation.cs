using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityAggregation : AbstractAggregationOperation {
        private readonly Entity _entity;
        private readonly string _separator;
        private readonly string[] _columnsToGroupBy;
        private readonly IDictionary<string, Field> _columnsToAccumulate;
        private readonly string _firstKey;
        private readonly IDictionary<string, StringBuilder> _builders = new Dictionary<string, StringBuilder>();
        private char[] _separatorArray;

        public EntityAggregation(Entity entity, string separator = ", ") {
            _entity = entity;
            _separator = separator;
            _separatorArray = separator.ToCharArray();
            _columnsToGroupBy = new FieldSqlWriter(_entity.All).ExpandXml().Input().Group().Context().Select(e => e.Value.Alias).ToArray();
            _firstKey = _columnsToGroupBy.Length > 0 ? _columnsToGroupBy[0] : _columnsToAccumulate.Select(c => c.Key).First();
            _columnsToAccumulate = new FieldSqlWriter(_entity.All).ExpandXml().Input().Aggregate().Context().ToDictionary(k => k.Key, v => v.Value);

            foreach (var pair in _columnsToAccumulate) {
                _builders[pair.Key] = new StringBuilder();
            }
        }

        protected override void Accumulate(Row row, Row aggregate) {
            //init
            if (!aggregate.ContainsKey(_firstKey)) {
                foreach (var column in _columnsToGroupBy) {
                    aggregate[column] = row[column];
                }

                foreach (var pair in _columnsToAccumulate) {
                    aggregate[pair.Value.Alias] = pair.Value.Default;
                }
            }
            //accumulate
            foreach (var pair in _columnsToAccumulate) {
                switch (pair.Value.Aggregate) {
                    case "sum":
                        switch (pair.Value.SimpleType) {
                            case "int32":
                                aggregate[pair.Key] = (int)aggregate[pair.Key] + (int)row[pair.Key];
                                break;
                            default:
                                break;
                        }
                        break;
                    case "join":
                        var aggregateValue = aggregate[pair.Key].ToString();
                        var aggregateIsEmpty = aggregateValue == string.Empty;

                        var rowValue = row[pair.Key].ToString();
                        var rowIsEmpty = rowValue == string.Empty;


                        if (aggregateIsEmpty && rowIsEmpty)
                            break;

                        if (!aggregateIsEmpty) {
                            _builders[pair.Key].Clear();
                            _builders[pair.Key].Append(aggregateValue);
                            if (!rowIsEmpty && aggregateValue != rowValue) {
                                _builders[pair.Key].Append(_separator);
                                _builders[pair.Key].Append(rowValue);
                            }
                        }

                        if (!rowIsEmpty) {
                            aggregate[pair.Key] = rowValue;
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
    }
}