using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {

    public class EntitiesLoader
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly EntityElementCollection _elements;

        public EntitiesLoader(ref Process process, EntityElementCollection elements)
        {
            _process = process;
            _elements = elements;
        }

        public void Load()
        {
            var count = 0;
            var batchId = _process.Options.Mode == Modes.Initialize ? 1 : _process.GetNextBatchId();

            foreach (EntityConfigurationElement element in _elements) {
                var reader = new EntityConfigurationLoader(_process);
                var entity = reader.Read(batchId, element, count == 0);

                GuardAgainstFieldOverlap(entity);

                _process.Entities.Add(entity);
                if (_process.MasterEntity == null)
                    _process.MasterEntity = entity;
                count++;
                batchId++;
            }

        }

        private void GuardAgainstFieldOverlap(Entity entity) {
            var entityKeys = new HashSet<string>(entity.Fields.OutputKeys());
            var processKeys = new HashSet<string>(_process.Entities.OutputKeys());
            entityKeys.IntersectWith(processKeys);

            if (!entityKeys.Any()) return;

            var count = entityKeys.Count;
            _log.Error("field overlap error in {3}.  The field{1}: {0} {2} already defined in previous entities.  You must alias (rename) these.", string.Join(", ", entityKeys), count.Plural(), count == 1 ? "is" : "are", entity.Alias);
            Environment.Exit(1);
        }
    }
}
