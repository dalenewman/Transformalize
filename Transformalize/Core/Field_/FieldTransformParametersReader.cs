using Transformalize.Configuration;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Process_;
using Transformalize.Core.Transform_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Field_
{
    public class FieldTransformParametersReader : ITransformParametersReader
    {
        private readonly string _name;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public FieldTransformParametersReader(string name)
        {
            _name = name;
        }

        public Parameters Read(TransformConfigurationElement transform)
        {
            var parameters = new Parameters();

            if (transform.Parameter != string.Empty && transform.Parameter != "*")
            {
                transform.Parameters.Insert(new ParameterConfigurationElement { Field = transform.Parameter });
            }

            foreach (ParameterConfigurationElement p in transform.Parameters)
            {
                if (string.IsNullOrEmpty(p.Name))
                {
                    _log.Warn("{0} | The field {1} has a {2} transform parameter without a name attribute.  Field parameters require names and values.", Process.Name,  _name, transform.Method);
                    return new Parameters();
                }

                if (string.IsNullOrEmpty(p.Value))
                {
                    _log.Warn("{0} | The field {1} has a {2} transform parameter without a value attribute.  Field parameters require names and values.", Process.Name, _name, transform.Method);
                    return new Parameters();
                }

                parameters.Add(p.Name, p.Name, p.Value, p.Type);
            }

            return parameters;

        }
    }
}