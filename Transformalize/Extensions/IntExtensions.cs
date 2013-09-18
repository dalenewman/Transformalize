namespace Transformalize.Extensions
{
    public static class IntExtensions
    {
        public static string Plural(this int i)
        {
            return i == 1 ? string.Empty : "s";
        }

        public static string Pluralize(this int i)
        {
            return i == 1 ? "y" : "ies";
        }
    }
}