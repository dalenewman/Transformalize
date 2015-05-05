using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Logging;

namespace Transformalize.Main {

    public class EntitiesLoader {

        private readonly Process _process;
        private readonly List<TflEntity> _elements;

        public EntitiesLoader(Process process, List<TflEntity> elements) {
            _process = process;
            _elements = elements;
        }

        public void Load() {
            short entityIndex = 0;
            foreach (var element in _elements) {
                var entity = new EntityConfigurationLoader(_process).Read(element, entityIndex);

                GuardAgainstFieldOverlap(entity);

                _process.Entities.Add(entity);
                if (_process.MasterEntity == null)
                    _process.MasterEntity = entity;

                entityIndex++;
            }
        }

        private void GuardAgainstFieldOverlap(Entity entity) {

            var entityKeys = new HashSet<string>(entity.Fields.WithOutput().WithoutPrimaryKey().Aliases());
            var processKeys = new HashSet<string>(_process.Entities.SelectMany(e2 => e2.Fields.WithOutput().Aliases()));

            entityKeys.IntersectWith(processKeys);

            if (!entityKeys.Any()) return;

            var count = entityKeys.Count;
            _process.Logger.EntityWarn(entity.Name, "field overlap error in {3}.  The field{1}: {0} {2} already defined in previous entities.  You must alias (rename) these.", string.Join(", ", entityKeys), count.Plural(), count == 1 ? "is" : "are", entity.Alias);
        }
    }
}
