using Transformalize.Core.Field_;
using Transformalize.Core.Parameters_;

namespace Transformalize.Core.Entity_
{
    public class EntityParametersReader : IParametersReader
    {
        private readonly Entity _entity;
        private readonly IParameters _parameters = new Parameters();

        public EntityParametersReader(Entity entity)
        {
            _entity = entity;
        }

        public IParameters Read()
        {
            var fields = new FieldSqlWriter(_entity.All).ExpandXml().Input().ToArray();
            foreach (var field in fields)
            {
                _parameters.Add(field.Alias, field.Alias, null, field.Type);
            }
            return _parameters;
        }
    }
}