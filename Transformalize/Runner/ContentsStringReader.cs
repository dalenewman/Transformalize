using System.Collections.Specialized;

namespace Transformalize.Runner {
    public class ContentsStringReader : ContentsReader {
        private readonly NameValueCollection _query;

        public ContentsStringReader(NameValueCollection query) {
            _query = query;
        }

        public override Contents Read(string resource) {
            return new Contents() {
                Content = ReplaceParameters(resource, _query),
                FileName = string.Empty,
                Name = string.Empty
            };
        }
    }
}