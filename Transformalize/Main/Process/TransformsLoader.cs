using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Main {
    public class TransformsLoader {
        private readonly Process _process;
        private readonly EntityElementCollection _entities;

        public TransformsLoader(ref Process process, EntityElementCollection entities) {
            _process = process;
            _entities = entities;
        }

        public void Load() {
            foreach (EntityConfigurationElement entityElement in _entities) {
                var entity = _process.Entities.First(e => e.Alias == entityElement.Alias);
                var transformFactory = new TransformOperationFactory(_process);

                foreach (FieldConfigurationElement fieldElement in entityElement.Fields) {
                    var alias = Common.GetAlias(fieldElement, true, entityElement.Prefix);
                    var field = _process.GetField(alias);
                    foreach (TransformConfigurationElement transformElement in fieldElement.Transforms) {
                        var parameters = new FieldTransformParametersReader().Read(transformElement);
                        entity.TransformOperations.Add(transformFactory.Create(field, transformElement, parameters));
                    }
                }

                foreach (FieldConfigurationElement calculatedElement in entityElement.CalculatedFields) {
                    var field = _process.GetField(calculatedElement.Alias);
                    foreach (TransformConfigurationElement transformElement in calculatedElement.Transforms) {
                        var parameters = transformElement.Parameter == "*" ? new EntityParametersReader(entity).Read() : new EntityTransformParametersReader(entity).Read(transformElement);
                        entity.TransformOperations.Add(transformFactory.Create(field, transformElement, parameters));
                    }
                }
            }
        }
    }
}