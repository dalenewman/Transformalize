using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Core.Fields_;
using Transformalize.Core.Process_;
using Transformalize.Core.Transform_;

namespace Transformalize.Core.Entity_
{
    public class EntityTransformReader : ITransformReader
    {
        private readonly Process _process;
        private readonly Entity _entity;
        private readonly TransformElementCollection _transforms;

        public EntityTransformReader(Process process, Entity entity, TransformElementCollection transforms)
        {
            _process = process;
            _entity = entity;
            _transforms = transforms;
        }

        public AbstractTransform[] Read()
        {

            var result = new List<AbstractTransform>();

            foreach (TransformConfigurationElement t in _transforms)
            {
                var parametersReader = new EntityTransformParametersReader(_entity, t);
                var fieldsReader = new FieldsReader(_process, _entity, t.Results);
                result.Add(new TransformFactory(_process, t, parametersReader, fieldsReader).Create());
            }

            return result.ToArray();
        }
    }
}