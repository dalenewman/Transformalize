using System.Collections.Generic;
using Pipeline.Configuration;

namespace Pipeline.Contracts {
    public interface IRunTimeRun {
        IEnumerable<IRow> Run(Process process);
    }
}