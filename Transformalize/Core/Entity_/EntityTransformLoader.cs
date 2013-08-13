using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Process_;
using Transformalize.Core.Transform_;

namespace Transformalize.Core.Entity_
{
    public class EntityTransformLoader : ITransformLoader
    {
        private readonly Process _process;
        private readonly Entity _entity;
        private readonly TransformElementCollection _transforms;

        public EntityTransformLoader(Process process, ref Entity entity, TransformElementCollection transforms)
        {
            _process = process;
            _entity = entity;
            _transforms = transforms;
        }

        public void Load()
        {

            foreach (TransformConfigurationElement t in _transforms)
            {
                var parametersReader = new EntityTransformParametersReader(_entity, t);

                if (t.Result != string.Empty)
                    t.Results.Insert(new FieldConfigurationElement() { Name = t.Result });

                var fieldsReader = new FieldsReader(_entity, t.Results);
                _entity.Transforms.Add(new TransformFactory(t, parametersReader, fieldsReader).Create());
            }

        }
    }
}