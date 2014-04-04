using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {

    public class EntitiesLoader {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly Process _process;
        private readonly EntityElementCollection _elements;

        public EntitiesLoader(ref Process process, EntityElementCollection elements) {
            _process = process;
            _elements = elements;
        }

        public void Load() {
            var count = 0;
            var batchId = _process.Options.Mode.Equals("init", IC) || !_process.OutputConnection.IsDatabase ? 1 : _process.GetNextBatchId();

            foreach (EntityConfigurationElement element in _elements) {
                var entity = new EntityConfigurationLoader(_process).Read(batchId, element, count == 0);

                GuardAgainstFieldOverlap(entity);

                _process.Entities.Add(entity);
                if (_process.MasterEntity == null)
                    _process.MasterEntity = entity;
                count++;
                batchId++;
            }

        }

        private void GuardAgainstFieldOverlap(Entity entity) {

            var entityKeys = new HashSet<string>(entity.Fields.ToEnumerable().Where(f => f.Output && !f.FieldType.HasFlag(FieldType.PrimaryKey)).Select(f => f.Alias));
            var processKeys = new HashSet<string>(_process.Entities.SelectMany(e2 => e2.Fields.ToEnumerable().Where(f => f.Output).Select(f => f.Alias)));

            entityKeys.IntersectWith(processKeys);

            if (!entityKeys.Any()) return;

            var count = entityKeys.Count;
            _log.Warn("field overlap error in {3}.  The field{1}: {0} {2} already defined in previous entities.  You must alias (rename) these.", string.Join(", ", entityKeys), count.Plural(), count == 1 ? "is" : "are", entity.Alias);
        }
    }
}
