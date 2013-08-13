using Transformalize.Configuration;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Transform_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Process_
{
    public class ProcessTransformParametersReader : ITransformParametersReader
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly TransformConfigurationElement _transform;
        private readonly char[] _dotArray = new[] { '.' };

        public ProcessTransformParametersReader(Process process, TransformConfigurationElement transform)
        {
            _process = process;
            _transform = transform;
        }

        public Parameters Read()
        {
            var parameters = new Parameters();

            if (_transform.Parameter != string.Empty)
            {
                if (_transform.Parameter.Contains("."))
                {
                    var values = _transform.Parameter.Split(_dotArray);
                    _transform.Parameters.Insert(new ParameterConfigurationElement { Entity = values[0], Field = values[1] });
                }
                else
                {
                    _transform.Parameters.Insert(new ParameterConfigurationElement { Entity = _transform.Parameter });
                }
            }

            foreach (ParameterConfigurationElement p in _transform.Parameters)
            {
                if (!string.IsNullOrEmpty(p.Field) && string.IsNullOrEmpty(p.Entity))
                {
                    _log.Warn("{0} | The process {1} has a {2} transform parameter with a field attribute but not an entity attribute.  If you have a field attribute, you must have an entity attribute.", Process.Name, _transform.Method);
                    return new Parameters();
                }

                if (string.IsNullOrEmpty(p.Field) && (string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value)))
                {
                    _log.Warn("{0} | The process {1} has a {2} transform parameter without a field and entity attributes, or name and value attributes.  Process parameters require one or the other.", Process.Name, _transform.Method);
                    return new Parameters();
                }

                if (!string.IsNullOrEmpty(p.Field))
                {
                    var fields = _process.InputFields();
                    if (fields.Any(Common.FieldFinder(p)))
                    {
                        var field = fields.Last(Common.FieldFinder(p)).Value;
                        var name = string.IsNullOrEmpty(p.Name) ? field.Alias : p.Name;
                        parameters.Add(field.Alias, name, null, field.Type);
                    }
                    else
                    {
                        _log.Warn("{0} | The process {1} has a {2} transform parameter that references field {3}.  This field doesn't exist.", Process.Name, _transform.Method, p.Field);
                        return new Parameters();
                    }
                }
                else
                {
                    parameters.Add(p.Name, p.Name, p.Value, p.Type);
                }
            }

            if (!parameters.Any())
            {
                foreach (var field in _process.InputFields().ToEnumerable())
                {
                    parameters.Add(field.Alias, field.Alias, null, field.Type);
                }
            }

            return parameters;

        }

    }
}