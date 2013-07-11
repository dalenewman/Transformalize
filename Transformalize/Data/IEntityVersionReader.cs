namespace Transformalize.Data {
    public interface IEntityVersionReader {
        bool IsRange { get; }
        bool HasRows { get; }
        object GetBeginVersion();
        object GetEndVersion();
        bool BeginAndEndAreEqual();
    }
}