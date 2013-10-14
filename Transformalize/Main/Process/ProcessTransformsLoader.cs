using Transformalize.Configuration;

namespace Transformalize.Main {
    public class ProcessTransformsLoader {
        private readonly Process _process;
        private readonly FieldElementCollection _elements;

        public ProcessTransformsLoader(ref Process process, FieldElementCollection elements) {
            _process = process;
            _elements = elements;
        }

        public void Load() {
            foreach (FieldConfigurationElement fieldElement in _elements) {
                var transformParametersReader = new ProcessTransformParametersReader(_process);
                var parametersReader = new ProcessParametersReader(_process);
                var fr = new FieldReader(_process, _process.MasterEntity, usePrefix: false);
                var field = fr.Read(fieldElement);
                _process.CalculatedFields.Add(fieldElement.Alias, field);

                foreach (TransformConfigurationElement transformElement in fieldElement.Transforms) {
                    var factory = new TransformOperationFactory(_process);
                    var parameters = transformElement.Parameter == "*" ? parametersReader.Read() : transformParametersReader.Read(transformElement);
                    _process.TransformOperations.Add(factory.Create(field, transformElement, parameters));
                    foreach (var parameter in parameters) {
                        _process.Parameters[parameter.Key] = parameter.Value;
                    }
                }
            }
        }

    }
}