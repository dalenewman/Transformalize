namespace Transformalize.Runner {
    public class Contents {
        public string Name { get; set; }
        public string Content { get; set; }
        public string FileName { get; set; }

        public Contents() {
            Name = string.Empty;
            Content = string.Empty;
            FileName = string.Empty;
        }
    }
}