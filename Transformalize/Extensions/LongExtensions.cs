namespace Transformalize.Extensions
{
    public static class LongExtensions
    {
        public static string Plural(this long i)
        {
            return i == 1 ? string.Empty : "s";
        }

        public static string Pluralize(this long i)
        {
            return i == 1 ? "y" : "ies";
        }
    }
}