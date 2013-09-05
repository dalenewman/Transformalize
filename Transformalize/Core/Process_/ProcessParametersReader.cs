using Transformalize.Core.Parameters_;

namespace Transformalize.Core.Process_
{
    public class ProcessParametersReader : IParametersReader
    {
        private readonly Process _process;
        private readonly IParameters _parameters = new Parameters();

        public ProcessParametersReader(Process process)
        {
            _process = process;
        }

        public IParameters Read()
        {
            foreach (var field in _process.OutputFields().ToEnumerable())
            {
                _parameters.Add(field.Alias, field.Alias, null, field.Type);
            }
            return _parameters;
        }
    }
}