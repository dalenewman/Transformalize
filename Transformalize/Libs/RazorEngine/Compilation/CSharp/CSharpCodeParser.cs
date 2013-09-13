#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Web.Razor.Generator;
using System.Web.Razor.Parser;
using System.Web.Razor.Text;
using Transformalize.Libs.RazorEngine.CodeGenerators;

namespace Transformalize.Libs.RazorEngine.Compilation.CSharp
{
    /// <summary>
    ///     Defines a code parser that supports the C# syntax.
    /// </summary>
    public class CSharpCodeParser : System.Web.Razor.Parser.CSharpCodeParser
    {
        #region Fields

        private const string GenericTypeFormatString = "{0}<{1}>";
        private SourceLocation? _endInheritsLocation;
        private bool _modelStatementFound;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="CSharpCodeParser" />.
        /// </summary>
        public CSharpCodeParser()
        {
            MapDirectives(ModelDirective, "model");
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Parses the inherits statement.
        /// </summary>
        protected override void InheritsDirective()
        {
            // Verify we're on the right keyword and accept
            AssertDirective(SyntaxConstants.CSharp.InheritsKeyword);
            AcceptAndMoveNext();
            _endInheritsLocation = CurrentLocation;

            InheritsDirectiveCore();
            CheckForInheritsAndModelStatements();
        }

        private void CheckForInheritsAndModelStatements()
        {
            if (_modelStatementFound && _endInheritsLocation.HasValue)
            {
                Context.OnError(_endInheritsLocation.Value, "The 'inherits' keyword is not allowed when a 'model' keyword is used.");
            }
        }

        /// <summary>
        ///     Parses the model statement.
        /// </summary>
        protected virtual void ModelDirective()
        {
            // Verify we're on the right keyword and accept
            AssertDirective("model");
            AcceptAndMoveNext();

            var endModelLocation = CurrentLocation;

            BaseTypeDirective("The 'model' keyword must be followed by a type name on the same line.", CreateModelCodeGenerator);

            if (_modelStatementFound)
            {
                Context.OnError(endModelLocation, "Only one 'model' statement is allowed in a file.");
            }

            _modelStatementFound = true;

            CheckForInheritsAndModelStatements();
        }

        private SpanCodeGenerator CreateModelCodeGenerator(string model)
        {
            return new SetModelTypeCodeGenerator(model, GenericTypeFormatString);
        }

        #endregion
    }
}