using Transformalize.Model;

namespace Transformalize.Runner
{
    public class ProcessRunner : AbstractProcessRunner
    {
        public ProcessRunner(Process process)
        {
            Process = process;
        }

        public ProcessRunner(Process process, Options options)
        {
            Process = process;
            Process.Options = options;
        }

        public new void Run()
        {
            base.Run();
        }

    }
}