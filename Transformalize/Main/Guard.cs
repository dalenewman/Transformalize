using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Main {
    public class Guard {
        public static bool Against(CfgProblems problems, bool assertion, string message, params object[] args) {
            if (assertion == false)
                return false;
            problems.AddCustomProblem(message, args);
            return true;
        }
    }
}
