namespace Transformalize.Data {
    public interface IEntityVersionWriter {
        void WriteEndVersion(object end, long count);
    }
}