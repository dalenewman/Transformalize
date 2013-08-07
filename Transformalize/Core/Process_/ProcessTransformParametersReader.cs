using System.Linq;
using Transformalize.Configuration;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
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

        public ProcessTransformParametersReader(Process process, TransformConfigurationElement transform)
        {
            _process = process;
            _transform = transform;
        }

        public Parameters Read()
        {
            var parameters = new Parameters();

            foreach (ParameterConfigurationElement p in _transform.Parameters)
            {
                if (!string.IsNullOrEmpty(p.Field) && string.IsNullOrEmpty(p.Entity))
                {
                    _log.Warn("{0} | The process {1} has a {2} transform parameter with a field attribute but not an entity attribute.  If you have a field attribute, you must have an entity attribute.", _process.Name, _transform.Method);
                    return new Parameters();
                }

                if (string.IsNullOrEmpty(p.Field) && (string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value)))
                {
                    _log.Warn("{0} | The process {1} has a {2} transform parameter without a field and entity attributes, or name and value attributes.  Process parameters require one or the other.", _process.Name, _transform.Method);
                    return new Parameters();
                }

                if (!string.IsNullOrEmpty(p.Field))
                {
                    var fields = _process.InputFields();
                    if (fields.Any(Common.FieldFinder(p)))
                    {
                        var field = fields.Last(Common.FieldFinder(p)).Value;
                        var name = string.IsNullOrEmpty(p.Name) ? field.Alias : p.Name;
                        parameters.Add(field.Alias, name, field.Default, field.Type);
                    }
                    else
                    {
                        _log.Warn("{0} | The process {1} has a {2} transform parameter that references field {3}.  This field doesn't exist.", _process.Name, _transform.Method, p.Field);
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
                    parameters.Add(field.Alias, field.Alias, field.Default, field.Type);
                }
            }

            return parameters;

        }

    }
}