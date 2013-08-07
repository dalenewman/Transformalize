using Transformalize.Configuration;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Transform_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Field_
{
    public class FieldTransformParametersReader : ITransformParametersReader
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Field _field;
        private readonly TransformConfigurationElement _transform;

        public FieldTransformParametersReader(Field field, TransformConfigurationElement transform)
        {
            _field = field;
            _transform = transform;
        }

        public Parameters Read()
        {
            var parameters = new Parameters();

            foreach (ParameterConfigurationElement p in _transform.Parameters)
            {
                if (string.IsNullOrEmpty(p.Name))
                {
                    _log.Warn("{0} | The field {1} in entity {2} has a {3} transform parameter without a name attribute.  Field parameters require names and values.", _field.Process,  _field.Alias, _field.Entity, _transform.Method);
                    return new Parameters();
                }

                if (string.IsNullOrEmpty(p.Value))
                {
                    _log.Warn("{0} | The field {1} in entity {2} has a {3} transform parameter without a value attribute.  Field parameters require names and values.", _field.Process, _field.Alias, _field.Entity, _transform.Method);
                    return new Parameters();
                }

                parameters.Add(p.Name, p.Name, p.Value, p.Type);
            }

            return parameters;

        }
    }
}