using System.Collections.Generic;
using Pipeline.Configuration;

namespace Pipeline.Contracts {
    public interface IRunTimeExecute {
        void Execute(Process process);
        void Execute(string cfg, string shorthand, Dictionary<string, string> parameters);
    }
}