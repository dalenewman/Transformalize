using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Process_;
using Transformalize.Core.Transform_;
using System.Linq;

namespace Transformalize.Core.Entity_
{
    public class EntityTransformLoader : ITransformLoader
    {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Entity _entity;
        private readonly TransformElementCollection _transforms;

        public EntityTransformLoader(ref Entity entity, TransformElementCollection transforms)
        {
            _entity = entity;
            _transforms = transforms;
        }

        public void Load()
        {

            foreach (TransformConfigurationElement element in _transforms)
            {
                if (IsTypeBasedTransform(element))
                {
                    PushTransformDownToField(element);
                }
                else
                {
                    var parametersReader = new EntityTransformParametersReader(_entity, element);

                    if (ResultIsNotSpecified(element))
                        element.Results.Insert(new FieldConfigurationElement() { Name = element.Result });

                    var fieldsReader = new FieldsReader(_entity, element.Results);
                    _entity.Transforms.Add(new TransformFactory(element, parametersReader, fieldsReader).Create());
                }
            }
        }

        private static bool ResultIsNotSpecified(TransformConfigurationElement element)
        {
            return element.Result != string.Empty;
        }

        private void PushTransformDownToField(TransformConfigurationElement element)
        {
            var t = element;
            foreach (var field in _entity.InputFields().ToEnumerable().Where(f => f.Type.Equals(t.Type, IC)))
            {
                var parametersReader = new FieldTransformParametersReader(field, t);
                var resultsReader = new FieldsReader(null, t.Results);
                field.Transforms.Add(new TransformFactory(element, parametersReader, resultsReader).Create(field.Alias));
            }
        }

        private static bool IsTypeBasedTransform(TransformConfigurationElement t)
        {
            return t.Type != string.Empty;
        }
    }
}