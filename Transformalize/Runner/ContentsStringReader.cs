namespace Transformalize.Runner {
    public class ContentsStringReader : ContentsReader {

        public override Contents Read(string resource) {
            return new Contents() {
                Content = resource,
                FileName = string.Empty,
                Name = "XML"
            };
        }
    }
}