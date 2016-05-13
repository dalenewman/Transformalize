using Pipeline.Configuration;

namespace Pipeline.Contracts {
    public interface IRunTimeSchemaReader : ISchemaReader {
        Process Process { get; set; }
        Schema Read(Process process);
    }
}