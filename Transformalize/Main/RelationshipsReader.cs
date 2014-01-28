using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {
    public class RelationshipsReader {

        private readonly Process _process;
        private readonly RelationshipElementCollection _elements;
        private readonly Logger _log = LogManager.GetLogger(string.Empty);

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public RelationshipsReader(Process process, RelationshipElementCollection elements) {
            _process = process;
            _elements = elements;
        }

        public List<Relationship> Read() {
            var relationships = new List<Relationship>();

            foreach (RelationshipConfigurationElement r in _elements) {
                var leftEntity = _process.Entities.First(e => e.Alias.Equals(r.LeftEntity, IC));
                var rightEntity = _process.Entities.First(e => e.Alias.Equals(r.RightEntity, IC));
                var join = GetJoins(r, leftEntity, rightEntity);
                var relationship = new Relationship {
                    LeftEntity = leftEntity,
                    RightEntity = rightEntity,
                    Join = join
                };

                relationships.Add(relationship);
            }
            return relationships;
        }

        private List<Join> GetJoins(RelationshipConfigurationElement r, Entity leftEntity, Entity rightEntity) {
            if (string.IsNullOrEmpty(r.LeftField)) {
                return (
                    from JoinConfigurationElement j in r.Join
                    select GetJoin(leftEntity, j.LeftField, rightEntity, j.RightField)
                ).ToList();
            }

            // if it's a single field join, you can use leftField and rightField on the relationship element
            return new List<Join> {
                GetJoin(leftEntity, r.LeftField, rightEntity, r.RightField)
            };
        }

        private Join GetJoin(Entity leftEntity, string leftField, Entity rightEntity, string rightField) {
            if (!leftEntity.Fields.ContainsKey(leftField) &&
                !leftEntity.Fields.ToEnumerable().Any(Common.FieldFinder(leftField))) {
                _log.Error("The left entity {0} does not have a field named {1} for joining to the right entity {2} with field {3}.",
                    leftEntity.Alias, leftField, rightEntity.Alias, rightField);
                Environment.Exit(1);
            }

            if (!rightEntity.Fields.ContainsKey(rightField) &&
                !rightEntity.Fields.ToEnumerable().Any(Common.FieldFinder(rightField))) {
                _log.Error(
                    "The right entity {0} does not have a field named {1} for joining to the left entity {2} with field {3}.",
                    rightEntity.Alias, rightField, leftEntity.Alias, leftField);
                Environment.Exit(1);
            }

            var join = new Join {
                LeftField =
                    leftEntity.Fields.ContainsKey(leftField)
                        ? leftEntity.Fields[leftField]
                        : leftEntity.Fields.ToEnumerable().First(Common.FieldFinder(leftField)),
                RightField =
                    rightEntity.Fields.ContainsKey(rightField)
                        ? rightEntity.Fields[rightField]
                        : rightEntity.Fields.ToEnumerable().First(Common.FieldFinder(rightField))
            };

            if (join.LeftField.FieldType.HasFlag(FieldType.MasterKey) || join.LeftField.FieldType.HasFlag(FieldType.PrimaryKey)) {
                join.LeftField.FieldType |= FieldType.ForeignKey;
            } else {
                join.LeftField.FieldType = FieldType.ForeignKey;
            }

            return join;
        }

    }
}
