namespace Transformalize.Data {
    public interface IConnectionChecker {
        bool Check(string connectionString);
    }
}