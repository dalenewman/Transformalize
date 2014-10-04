using Transformalize.Libs.DBDiff.Schema.Model;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces
{
    public interface ISQLServerSchemaBase
    {
        SchemaList<ExtendedProperty, ISchemaBase> ExtendedProperties { get; }
    }
}
