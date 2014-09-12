using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Runner {
    public interface IProcessRunner : IDisposable {
        IEnumerable<Row> Run(ref Process process);
        void SetLog(ref Process process);
    }
}