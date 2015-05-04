using Transformalize.Logging;

namespace Transformalize.Runner {
    public abstract class ContentsReader {
        public abstract Contents Read(string resource);
    }
}