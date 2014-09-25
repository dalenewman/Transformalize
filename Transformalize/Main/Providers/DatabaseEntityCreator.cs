using System;
using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Main.Providers {
    public class DatabaseEntityCreator : IEntityCreator {

        public IEntityExists EntityExists { get; set; }

        protected Fields GetRelationshipFields(IEnumerable<Relationship> rel, Entity entity) {

            var relationships = rel.Where(r => r.LeftEntity.Alias != entity.Alias && r.RightEntity.Alias != entity.Alias).ToArray();
            var fields = new Fields();
            if (relationships.Any()) {
                foreach (var relationship in relationships) {
                    var leftSide = relationship.LeftEntity.RelationshipToMaster.Count();
                    var rightSide = relationship.RightEntity.RelationshipToMaster.Count();
                    if (leftSide <= rightSide) {
                        foreach (var join in relationship.Join) {
                            fields.Add(join.LeftField);
                        }
                    } else {
                        foreach (var join in relationship.Join) {
                            fields.Add(join.RightField);
                        }
                    }
                }
            }
            return fields;
        }

        public virtual void Create(AbstractConnection connection, Process process, Entity entity) {
            throw new NotImplementedException();
        }

    }
}