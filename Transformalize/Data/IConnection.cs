namespace Transformalize.Data
{
    public interface IConnection {
        string Provider { get; set; }
        int BatchSize { get; set; }
        string ConnectionString { get; }
        bool IsReady();
        string Database { get; }
        string Server { get; }
        int CompatibilityLevel { get; }
    }
}