using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;
using Transformalize.Configuration;

namespace Transformalize.Main {
    public class RelationshipsReader {

        private readonly Process _process;
        private readonly RelationshipElementCollection _elements;

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public RelationshipsReader(Process process, RelationshipElementCollection elements) {
            _process = process;
            _elements = elements;
        }

        public List<Relationship> Read() {
            var relationships = new List<Relationship>();

            foreach (RelationshipConfigurationElement r in _elements) {
                Entity leftEntity;
                if (!_process.Entities.TryFind(r.LeftEntity, out leftEntity)) {
                    throw new TransformalizeException("Can't find left entity {0}.", r.LeftEntity);
                }

                Entity rightEntity;
                if (!_process.Entities.TryFind(r.RightEntity, out rightEntity)) {
                    throw new TransformalizeException("Can't find right entity {0}.", r.RightEntity);
                }

                var join = GetJoins(r, leftEntity, rightEntity);
                var relationship = new Relationship {
                    LeftEntity = leftEntity,
                    RightEntity = rightEntity,
                    Index = r.Index,
                    Join = join
                };

                relationships.Add(relationship);
            }
            return relationships;
        }

        private static List<Join> GetJoins(RelationshipConfigurationElement r, Entity leftEntity, Entity rightEntity) {
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

        private static Join GetJoin(Entity leftEntity, string leftField, Entity rightEntity, string rightField) {

            var leftFields = leftEntity.OutputFields();
            var rightFields = rightEntity.OutputFields();
            var leftHit = leftFields.HaveField(leftEntity.Alias, leftField);
            var rightHit = rightFields.HaveField(rightEntity.Alias, rightField);

            if (!leftHit) {
                throw new TransformalizeException("The left entity {0} does not have a field named {1} for joining to the right entity {2} with field {3}.", leftEntity.Alias, leftField, rightEntity.Alias, rightField);
            }

            if (!rightHit) {
                throw new TransformalizeException("The right entity {0} does not have a field named {1} for joining to the left entity {2} with field {3}.", rightEntity.Alias, rightField, leftEntity.Alias, leftField);
            }

            var join = new Join {
                LeftField = leftFields.Find(leftEntity.Alias, leftField).First(),
                RightField = rightFields.Find(rightEntity.Alias, rightField).First()
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
