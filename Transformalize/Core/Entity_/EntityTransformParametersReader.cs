using Transformalize.Configuration;
using Transformalize.Core.Field_;
using Transformalize.Core.Parameter_;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Transform_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Entity_
{
    public class EntityTransformParametersReader : ITransformParametersReader
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Entity _entity;
        private readonly TransformConfigurationElement _transform;

        public EntityTransformParametersReader(Entity entity, TransformConfigurationElement transform)
        {
            _entity = entity;
            _transform = transform;
        }

        public Parameters Read()
        {
            var parameters = new Parameters();

            if (_transform.Parameter != string.Empty)
            {
                _transform.Parameters.Insert(new ParameterConfigurationElement {Entity = _entity.Name, Field = _transform.Parameter});
            }

            foreach (ParameterConfigurationElement p in _transform.Parameters)
            {
                if (string.IsNullOrEmpty(p.Field) && (string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value)))
                {
                    _log.Warn("{0} | The entity {1} has a {2} transform parameter without a field attribute, or name and value attributes.  Entity parameters require one or the other.", _entity.ProcessName, _entity.Name, _transform.Method);
                    return new Parameters();
                }

                if (!string.IsNullOrEmpty(p.Field))
                {
                    var fields = new FieldSqlWriter(_entity.All, _entity.Transforms.Results()).Input().ExpandXml().Context();
                    if (fields.Any(Common.FieldFinder(p)))
                    {
                        var field = fields.Last(Common.FieldFinder(p)).Value;
                        var key = string.IsNullOrEmpty(p.Name) ? field.Alias : p.Name;
                        parameters.Add(field.Alias, key, null, field.Type);
                    }
                    else
                    {
                        _log.Warn("{0} | The entity {1} has a {2} transform parameter that references field {3}.  This field doesn't exist in {1}.", _entity.ProcessName, _entity.Name, _transform.Method, p.Field);
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
                var fields = new FieldSqlWriter(_entity.All).ExpandXml().Input().Context().ToEnumerable();
                foreach (var field in fields)
                {
                    parameters.Add(field.Alias, field.Alias, null, field.Type);
                }
            }

            return parameters;

        }

    }
}