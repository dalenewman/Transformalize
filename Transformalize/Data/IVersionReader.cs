namespace Transformalize.Data {
    public interface IVersionReader {
        bool IsRange { get; }
        bool HasRows { get; }
        object GetBeginVersion();
        object GetEndVersion();
    }
}