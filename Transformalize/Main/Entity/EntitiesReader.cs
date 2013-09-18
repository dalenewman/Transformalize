using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {
    public class EntitiesReader
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly EntityElementCollection _elements;

        public EntitiesReader(ref Process process, EntityElementCollection elements)
        {
            _process = process;
            _elements = elements;
        }

        public Entities Read()
        {
            var entities = new Entities();
            var count = 0;

            foreach (EntityConfigurationElement element in _elements) {
                var reader = new EntityConfigurationReader(_process);
                var entity = reader.Read(element, count == 0);

                GuardAgainstFieldOverlap(entity);

                entities.Add(entity);
                if (entity.IsMaster())
                    _process.MasterEntity = entity;
                count++;
            }
            return entities;

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
