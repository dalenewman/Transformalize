using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Core.Fields_;
using Transformalize.Core.Process_;
using Transformalize.Core.Transform_;

namespace Transformalize.Core.Field_
{
    public class FieldTransformReader : ITransformReader
    {
        private readonly Process _process;
        private readonly Field _field;
        private readonly TransformElementCollection _transforms;

        public FieldTransformReader(Process process, Field field, TransformElementCollection transforms)
        {
            _process = process;
            _field = field;
            _transforms = transforms;
        }

        public AbstractTransform[] Read()
        {

            var result = new List<AbstractTransform>();

            foreach (TransformConfigurationElement t in _transforms)
            {
                var parametersReader = new FieldTransformParametersReader(_field, t);
                var fieldsReader = new FieldsReader(_process, null, t.Results);
                result.Add(new TransformFactory(_process, t, parametersReader, fieldsReader).Create(_field.Alias));
            }

            return result.ToArray();
        }
    }
}