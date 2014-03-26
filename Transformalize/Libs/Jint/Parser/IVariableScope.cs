using System.Collections.Generic;
using Transformalize.Libs.Jint.Parser.Ast;

namespace Transformalize.Libs.Jint.Parser
{
    /// <summary>
    /// Used to safe references to all variable delcarations in a specific scope.
    /// Hoisting.
    /// </summary>
    public interface IVariableScope
    {
        IList<VariableDeclaration> VariableDeclarations { get; set; }
    }

    public class VariableScope : IVariableScope
    {
        public VariableScope()
        {
            VariableDeclarations = new List<VariableDeclaration>();
        }

        public IList<VariableDeclaration> VariableDeclarations { get; set; }
    }
}