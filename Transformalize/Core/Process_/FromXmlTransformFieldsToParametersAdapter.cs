using System;
using Transformalize.Configuration;

namespace Transformalize.Core.Process_
{
    public class FromXmlTransformFieldsToParametersAdapter
    {
        private readonly ProcessConfigurationElement _process;

        public FromXmlTransformFieldsToParametersAdapter(ProcessConfigurationElement process)
        {
            _process = process;
        }

        public void Adapt()
        {
            foreach (EntityConfigurationElement entity in _process.Entities)
            {
                foreach (FieldConfigurationElement field in entity.Fields)
                {
                    foreach (TransformConfigurationElement transform in field.Transforms)
                    {
                        if (!transform.Method.Equals("fromxml", StringComparison.OrdinalIgnoreCase)) continue;

                        foreach (FieldConfigurationElement tField in transform.Fields)
                        {
                            transform.Parameters.Add(new ParameterConfigurationElement() { Entity = entity.Alias, Field = tField.Alias, Name = tField.Name});
                        }
                    }
                }
            }
        }
    }
}