using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Runner {
    public abstract class ContentsReader : WithLoggingMixin {
        public abstract Contents Read(string resource);
    }
}