namespace Transformalize.Libs.Sqloogle.Utilities
{
    public static class Enums
    {
        public enum DirectoryType
        {
            FileSystem,
            Memory
        }

        public enum WriteType
        {
            None,
            Create,
            Alter,
            Drop,
            Move
        }

    }
}
