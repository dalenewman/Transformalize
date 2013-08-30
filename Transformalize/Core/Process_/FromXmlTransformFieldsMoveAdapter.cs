using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Core.Process_
{
    public class FromXmlTransformFieldsMoveAdapter
    {
        private readonly ProcessConfigurationElement _process;

        public FromXmlTransformFieldsMoveAdapter(ProcessConfigurationElement process)
        {
            _process = process;
        }

        public void Adapt()
        {
            var fields = new Dictionary<string, Dictionary<string, List<FieldConfigurationElement>>>();

            foreach (EntityConfigurationElement entity in _process.Entities)
            {
                fields[entity.Alias] = new Dictionary<string, List<FieldConfigurationElement>>();

                foreach (FieldConfigurationElement field in entity.Fields)
                {
                    foreach (TransformConfigurationElement transform in field.Transforms)
                    {
                        if (!transform.Method.Equals("fromxml", StringComparison.OrdinalIgnoreCase)) continue;

                        fields[entity.Alias][field.Alias] = new List<FieldConfigurationElement>();
                        foreach (FieldConfigurationElement tField in transform.Fields)
                        {
                            tField.Input = false;
                            fields[entity.Alias][field.Alias].Add(tField);
                        }

                        transform.Fields.Clear();
                    }
                }
            }

            foreach (var entity in fields)
            {
                foreach (var field in entity.Value)
                {
                    var entityElement = _process.Entities.Cast<EntityConfigurationElement>().First(e => e.Alias == entity.Key);
                    var fieldElement = entityElement.Fields.Cast<FieldConfigurationElement>().First(f => f.Alias == field.Key);
                    var index = entityElement.Fields.IndexOf(fieldElement)+1;
                    foreach (var element in field.Value)
                    {
                        entityElement.Fields.InsertAt(element, index);
                        index++;
                    }
                }
            }

        }
    }
}