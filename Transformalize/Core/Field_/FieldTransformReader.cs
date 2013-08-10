using Transformalize.Configuration;
using Transformalize.Core.Fields_;
using Transformalize.Core.Process_;
using Transformalize.Core.Transform_;

namespace Transformalize.Core.Field_
{
    public class FieldTransformReader : ITransformReader
    {
        private readonly Field _field;
        private readonly TransformElementCollection _transforms;

        public FieldTransformReader(Field field, TransformElementCollection transforms)
        {
            _field = field;
            _transforms = transforms;
        }

        public Transforms Read()
        {

            var transforms = new Transforms();

            foreach (TransformConfigurationElement t in _transforms)
            {
                var parametersReader = new FieldTransformParametersReader(_field, t);
                var fieldsReader = new FieldsReader(null, t.Results);
                transforms.Add(new TransformFactory(t, parametersReader, fieldsReader).Create(_field.Alias));
            }

            return transforms;
        }
    }
}