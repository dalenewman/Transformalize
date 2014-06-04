using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers.ElasticSearch;

namespace Transformalize.Main.Providers.Solr
{
    public class SolrEntityOutputKeysExtract : AbstractOperation {

        private readonly Entity _entity;
        private readonly List<AliasType> _aliasTypes = new List<AliasType>();
        private readonly string[] _sourceInclude;
        private readonly Dictionary<string, Func<object, object>> _conversionMap = Common.GetObjectConversionMap();

        public SolrEntityOutputKeysExtract(AbstractConnection connection, Entity entity) {
            _entity = entity;
            _aliasTypes = _entity.PrimaryKey.AliasTypes().ToList();
            if (_entity.Version != null) {
                _aliasTypes.Add(new AliasType() { Alias = _entity.Version.Alias, AliasLower = _entity.Version.Alias.ToLower(), SimpleType = _entity.Version.SimpleType });
            }
            _sourceInclude = _aliasTypes.Select(at => at.AliasLower).ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            throw new NotImplementedException();
        }
    }
}