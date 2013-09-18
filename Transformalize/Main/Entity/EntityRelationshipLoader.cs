using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {
    public class EntityRelationshipLoader {
        private readonly Process _process;

        public EntityRelationshipLoader(ref Process process) {
            _process = process;
        }

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public void Load() {
            foreach (var entity in _process.Entities) {
                entity.RelationshipToMaster = ReadRelationshipToMaster(entity);
                if (!entity.RelationshipToMaster.Any() && !entity.IsMaster()) {
                    _log.Error("The entity {0} must have a relationship to the master entity {1}.", entity.Name, _process.MasterEntity.Name);
                    Environment.Exit(1);
                }
            }
        }

        private IEnumerable<Relationship> ReadRelationshipToMaster(Entity rightEntity) {
            var relationships = _process.Relationships.Where(r => r.RightEntity.Equals(rightEntity)).ToList();

            if (relationships.Any() && !relationships.Any(r => r.LeftEntity.IsMaster())) {
                var leftEntity = relationships.Last().LeftEntity;
                relationships.AddRange(ReadRelationshipToMaster(leftEntity));
            }
            return relationships;
        }
    }
}
