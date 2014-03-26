using System;

namespace Transformalize.Libs.Jint.Parser
{
    public class ParserError : Exception
    {
        public int Column;
        public string Description;
        public int Index;
        public int LineNumber;

        public ParserError(string message) : base(message)
        {
        }
    }
}