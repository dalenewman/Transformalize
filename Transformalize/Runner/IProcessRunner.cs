using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Runner {
    public interface IProcessRunner : IDisposable {
        IEnumerable<Row> Run(Process process);
        void SetLog(Process process);
    }
}