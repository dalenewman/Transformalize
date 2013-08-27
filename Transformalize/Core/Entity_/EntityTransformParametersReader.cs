using Transformalize.Configuration;
using Transformalize.Core.Field_;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Transform_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Entity_
{
    public class EntityTransformParametersReader : ITransformParametersReader
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Entity _entity;

        public EntityTransformParametersReader(Entity entity)
        {
            _entity = entity;
        }

        public Parameters Read(TransformConfigurationElement transform)
        {
            var parameters = new Parameters();

            if (transform.Parameter != string.Empty && transform.Parameter != "*")
            {
                transform.Parameters.Insert(new ParameterConfigurationElement {Entity = _entity.Alias, Field = transform.Parameter});
            }

            foreach (ParameterConfigurationElement p in transform.Parameters)
            {
                if (string.IsNullOrEmpty(p.Field) && (string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value)))
                {
                    _log.Warn("{0} | The entity {1} has a {2} transform parameter without a field attribute, or name and value attributes.  Entity parameters require one or the other.", _entity.ProcessName, _entity.Alias, transform.Method);
                    return new Parameters();
                }

                if (!string.IsNullOrEmpty(p.Field))
                {
                    var fields = new FieldSqlWriter(_entity.All, _entity.CalculatedFields).Input().ExpandXml().Context();
                    if (fields.Any(Common.FieldFinder(p)))
                    {
                        var field = fields.Last(Common.FieldFinder(p));
                        var key = string.IsNullOrEmpty(p.Name) ? field.Key : p.Name;
                        parameters.Add(field.Key, key, null, field.Value.Type);
                    }
                    else
                    {
                        _log.Warn("{0} | The entity {1} has a {2} transform parameter that references field {3}.  This field doesn't exist in {1}.", _entity.ProcessName, _entity.Alias, transform.Method, p.Field);
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

    }
}