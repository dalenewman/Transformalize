namespace Transformalize.Model
{
    public class Script
    {
        public string Content { get; private set; }
        public bool IsTemplate { get; private set; }
        public string Name { get; private set; }
        public string File { get; private set; }

        public Script(string name, string content, string file, bool isTemplate = false)
        {
            File = file;
            Name = name;
            Content = content;
            IsTemplate = isTemplate;
        }

    }
}