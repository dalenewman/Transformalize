namespace Transformalize.Libs.Cfg.Net {
    public static class CharExtensions {
        public static bool IsVowel(this char c) {
            return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'A' || c == 'E' || c == 'I' ||
                   c == 'O' || c == 'U';
        }
    }
}