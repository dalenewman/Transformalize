using Transformalize.Configuration;

namespace Transformalize.Main {

    public class ProcessOperationsLoader {

        private readonly Process _process;
        private readonly FieldElementCollection _elements;

        public ProcessOperationsLoader(ref Process process, FieldElementCollection elements) {
            _process = process;
            _elements = elements;
        }

        public void Load() {

            foreach (FieldConfigurationElement f in _elements) {
                var field = new FieldReader(_process, _process.MasterEntity, false).Read(f);
                field.Input = false;
                _process.CalculatedFields.Add(f.Alias, field);

                foreach (TransformConfigurationElement t in f.Transforms) {

                    var factory = new TransformOperationFactory(_process);
                    var parameters = t.Parameter == "*" ?
                        new ProcessParametersReader(_process).Read() :
                        new ProcessTransformParametersReader(_process).Read(t);
                    var operation = factory.Create(field, t, parameters);

                    _process.TransformOperations.Add(operation);
                    foreach (var parameter in parameters) {
                        _process.Parameters[parameter.Key] = parameter.Value;
                    }
                }
            }
        }

    }
}