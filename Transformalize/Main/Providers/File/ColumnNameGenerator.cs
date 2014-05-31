using System;

namespace Transformalize.Main.Providers.File
{
    public static class ColumnNameGenerator {

        public static string CreateDefaultColumnName(int index) {
            var name = Convert.ToString((char)('A' + (index % 26)));
            while (index >= 26) {
                index = (index / 26) - 1;
                name = Convert.ToString((char)('A' + (index % 26))) + name;
            }
            return name;
        }
    }
}