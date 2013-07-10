namespace Transformalize.Data {
    public interface IEntityVersion {
        bool IsRange { get; }
        bool HasRows { get; }
        object GetBeginVersion();
        object GetEndVersion();
        bool BeginAndEndAreEqual();
    }
}