namespace Transformalize.Core.Parameters_
{
    public class EmptyParametersReader : IParametersReader
    {
        public IParameters Read()
        {
            return new Parameters();
        }
    }
}