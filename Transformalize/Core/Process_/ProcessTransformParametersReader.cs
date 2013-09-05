using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Core.Field_;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Transform_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Process_
{
    public class ProcessTransformParametersReader : ITransformParametersReader
    {
        private readonly Process _process;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly char[] _dotArray = new[] { '.' };
        private readonly Field[] _fields;

        public ProcessTransformParametersReader(Process process)
        {
            _process = process;
            _fields = _process.OutputFields().ToEnumerable().ToArray();
        }


        public Parameters Read(TransformConfigurationElement transform)
        {
            var parameters = new Parameters();

            if (transform.Parameter != string.Empty && transform.Parameter != "*")
            {
                AddParameterToConfiguration(transform, transform.Parameter, true);
            }

            if (transform.Method.ToLower() == "map")
            {
                AddMapParametersToConfiguration(transform, _process.MapEquals[transform.Map]);
                AddMapParametersToConfiguration(transform, _process.MapStartsWith[transform.Map]);
                AddMapParametersToConfiguration(transform, _process.MapEndsWith[transform.Map]);
            }

            foreach (ParameterConfigurationElement p in transform.Parameters)
            {
                //if (!string.IsNullOrEmpty(p.Field) && string.IsNullOrEmpty(p.Entity))
                //{
                //    _log.Warn("A process transform with {0} method has a parameter with a field attribute but not an entity attribute.  If you have a field attribute, you must have an entity attribute.", _transform.Method);
                //    return new Parameters();
                //}

                //if (string.IsNullOrEmpty(p.Field) && (string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value)))
                //{
                //    _log.Warn("The process transform with {0} method has a parameter without a field and entity attributes, or name and value attributes.  Process parameters require one or the other.", _transform.Method);
                //    return new Parameters();
                //}

                if (!string.IsNullOrEmpty(p.Field))
                {
                    var fields = _process.OutputFields();
                    if (fields.Any(Common.FieldFinder(p)))
                    {
                        var field = fields.Last(Common.FieldFinder(p)).Value;
                        var name = string.IsNullOrEmpty(p.Name) ? field.Alias : p.Name;
                        parameters.Add(field.Alias, name, null, field.Type);
                    }
                    else
                    {
                        _log.Warn("A {0} transform references {1}, but I can't find the definition for {1}.", transform.Method, p.Field);
                        return new Parameters();
                    }
                }
                else
                {
                    parameters.Add(p.Name, p.Name, p.Value, p.Type);
                }
            }

            return parameters;

        }

        private void AddParameterToConfiguration(TransformConfigurationElement transform, string parameter, bool insert)
        {
            try
            {
                if (parameter.Contains("."))
                {
                    var values = parameter.Split(_dotArray);
                    var p = new ParameterConfigurationElement {Entity = values[0], Field = values[1]};

                    if (insert)
                        transform.Parameters.Insert(p);
                    else
                        transform.Parameters.Add(p);
                }
                else
                {
                    var p = new ParameterConfigurationElement { Field = parameter };
                    if(insert)
                        transform.Parameters.Insert(p);
                    else
                        transform.Parameters.Add(p);
                }
            }
            catch (Exception)
            {
                _log.Warn("Process parameter {0} is already defined.  This could happen if you have a parameter attribute defined in your transform element, and also in your transform parameters collection.  Or, it could happen if you're using a map transform and your map output already references the parameters.", parameter);
            }
        }

        private void AddMapParametersToConfiguration(TransformConfigurationElement transform, IEnumerable<KeyValuePair<string, Item>> items)
        {
            foreach (var item in items)
            {
                if (item.Value.UseParameter)
                {
                    if (_fields.Any(Common.FieldFinder(item.Value.Parameter)))
                    {
                        item.Value.Parameter = _fields.First(Common.FieldFinder(item.Value.Parameter)).Alias;
                        AddParameterToConfiguration(transform, item.Value.Parameter, false);
                    }
                    else
                    {
                        _log.Error("The map parameter {0} does not exist.  Please make sure it matches a field's name or alias.", item.Value.Parameter);
                        Environment.Exit(0);
                    }
                }
            }            
        }

    }
}