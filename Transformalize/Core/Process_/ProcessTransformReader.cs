using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Core.Fields_;
using Transformalize.Core.Transform_;

namespace Transformalize.Core.Process_
{
    public class ProcessTransformReader : ITransformReader
    {
        private readonly Process _process;
        private readonly TransformElementCollection _transforms;

        public ProcessTransformReader(Process process, TransformElementCollection transforms)
        {
            _process = process;
            _transforms = transforms;
        }

        public AbstractTransform[] Read()
        {

            var result = new List<AbstractTransform>();

            foreach (TransformConfigurationElement t in _transforms)
            {
                var parametersReader = new ProcessTransformParametersReader(_process, t);
                var fieldsReader = new FieldsReader(_process, null, t.Results);
                result.Add(new TransformFactory(_process, t, parametersReader, fieldsReader).Create());
            }

            return result.ToArray();
        }
    }
}