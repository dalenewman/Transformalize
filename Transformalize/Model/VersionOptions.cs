namespace Transformalize.Model
{
    public class VersionOptions
    {
        public bool UseBeginVersion { get; set; }
        public bool WriteEndVersion { get; set; }

        public VersionOptions()
        {
            //defaults
            UseBeginVersion = true;
            WriteEndVersion = true;
        }
    }
}