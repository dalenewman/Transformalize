using System.Collections.Generic;
using Transformalize.Libs.Jint.Parser.Ast;

namespace Transformalize.Libs.Jint.Parser
{
    public interface IFunctionDeclaration : IFunctionScope
    {
        Identifier Id { get; }
        IEnumerable<Identifier> Parameters { get; }
        Statement Body { get; }
        bool Strict { get; }
    }
}