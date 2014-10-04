namespace Transformalize.Libs.DBDiff.Schema.Model
{
    public interface IDatabase:ISchemaBase
    {
        bool IsCaseSensity { get; }
        SqlAction ActionMessage { get; }
    }
}
