using System.Collections.Generic;

namespace Transformalize.Main {
    public class Guard {
        public static bool Against(List<string> problems, bool assertion, string message, params object[] args) {
            if (assertion == false)
                return false;
            problems.Add(string.Format(message, args));
            return true;
        }
    }
}
