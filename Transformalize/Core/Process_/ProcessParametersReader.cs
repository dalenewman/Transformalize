using Transformalize.Core.Parameters_;

namespace Transformalize.Core.Process_
{
    public class ProcessParametersReader : IParametersReader
    {
        private readonly IParameters _parameters = new Parameters();

        public IParameters Read()
        {
            foreach (var field in Process.InputFields().ToEnumerable())
            {
                _parameters.Add(field.Alias, field.Alias, null, field.Type);
            }
            return _parameters;
        }
    }
}