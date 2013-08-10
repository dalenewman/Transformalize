using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Core.Fields_;
using Transformalize.Core.Transform_;

namespace Transformalize.Core.Process_
{
    public class ProcessTransformLoader : ITransformLoader
    {
        private readonly Process _process;
        private readonly TransformElementCollection _transforms;

        public ProcessTransformLoader(ref Process process, TransformElementCollection transforms)
        {
            _process = process;
            _transforms = transforms;
        }

        public void Load()
        {

            foreach (TransformConfigurationElement t in _transforms)
            {
                var parametersReader = new ProcessTransformParametersReader(_process, t);
                var fieldsReader = new FieldsReader(null, t.Results);
                _process.Transforms.Add(new TransformFactory(t, parametersReader, fieldsReader).Create());
            }

        }
    }
}