namespace Transformalize.Readers {
    public interface IConfigurationReader<out T> {
        T Read();
        int Count { get; }
    }
}