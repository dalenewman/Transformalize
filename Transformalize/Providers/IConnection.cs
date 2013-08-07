namespace Transformalize.Providers
{
    public interface IConnection {
        int BatchSize { get; set; }
        string ConnectionString { get; }
        bool IsReady();
        string Database { get; }
        string Server { get; }
        int CompatibilityLevel { get; }
        ConnectionType ConnectionType { get; set; }
    }
}