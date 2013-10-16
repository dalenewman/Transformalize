using System.Linq;
using Transformalize.Configuration;
using Transformalize.Operations.Validate;

namespace Transformalize.Main
{
    public class ValidatorsLoader {

        private readonly Process _process;
        private readonly EntityElementCollection _entities;

        public ValidatorsLoader(ref Process process, EntityElementCollection entities) {
            _process = process;
            _entities = entities;
        }

        public void Load() {
            foreach (EntityConfigurationElement entityElement in _entities) {
                var entity = _process.Entities.First(e => e.Alias == entityElement.Alias);
                var factory = new TransformOperationFactory(_process);

                foreach (FieldConfigurationElement fieldElement in entityElement.Fields) {
                    var alias = Common.GetAlias(fieldElement, true, entityElement.Prefix);
                    var field = _process.GetField(alias);
                    foreach(TransformConfigurationElement validatorElement in fieldElement.Validators) {
                        entity.TransformOperations.Add(factory.Create(field, validatorElement, new Parameters.Parameters()));
                    }
                }
            }
        }
    }
}