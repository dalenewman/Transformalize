using System;
using System.Collections.Generic;

namespace Transformalize.Contracts {
    public interface IOperation : IDisposable {
        IContext Context { get; }
        bool Run { get; set; }
        void Error(string error);
        void Warn(string warning);
        IEnumerable<string> Errors();
        IEnumerable<string> Warnings();
        IRow Operate(IRow row);
        IEnumerable<IRow> Operate(IEnumerable<IRow> rows);
        uint RowCount { get; set; }
        void Increment();
    }
}