using System;
using Transformalize.Main;

namespace Transformalize.Runner
{
    public interface IProcessRunner : IDisposable {
        void Run(Process process);
    }
}