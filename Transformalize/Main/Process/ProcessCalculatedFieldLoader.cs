using Transformalize.Configuration;

namespace Transformalize.Main {
    public class ProcessCalculatedFieldLoader {
        private readonly Process _process;
        private readonly FieldElementCollection _elements;

        public ProcessCalculatedFieldLoader(ref Process process, FieldElementCollection elements) {
            _process = process;
            _elements = elements;
        }


        // this has to be loader, not a reader, because calculated fields depend on calculated fields as parameters
        public void Load() {
            foreach (FieldConfigurationElement field in _elements) {
                var transformParametersReader = new ProcessTransformParametersReader(_process);
                var parametersReader = new ProcessParametersReader(_process);
                var fr = new FieldReader(_process, _process.MasterEntity, transformParametersReader, parametersReader, usePrefix:false);
                _process.CalculatedFields.Add(field.Alias, fr.Read(field));
            }
        }
    }
}
