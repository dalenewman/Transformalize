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

using System;
using System.Linq;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;
using System.Web.Razor.Tokenizer.Symbols;
using Transformalize.Libs.RazorEngine.CodeGenerators;

namespace Transformalize.Libs.RazorEngine.Compilation.VisualBasic
{
    /// <summary>
    ///     Defines a code parser that supports the VB syntax.
    /// </summary>
    public class VBCodeParser : System.Web.Razor.Parser.VBCodeParser
    {
        #region Fields

        private const string GenericTypeFormatString = "{0}(Of {1})";
        private SourceLocation? _endInheritsLocation;
        private bool _modelStatementFound;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="VBCodeParser" />
        /// </summary>
        public VBCodeParser()
        {
            MapDirective("ModelType", ModelTypeDirective);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Parses the inherits statement.
        /// </summary>
        protected override bool InheritsStatement()
        {
            // Verify we're on the right keyword and accept
            Assert(VBKeyword.Inherits);
            var inherits = CurrentSymbol;
            NextToken();
            _endInheritsLocation = CurrentLocation;
            PutCurrentBack();
            PutBack(inherits);
            EnsureCurrent();

            var result = base.InheritsStatement();
            CheckForInheritsAndModelStatements();
            return result;
        }

        private void CheckForInheritsAndModelStatements()
        {
            if (_modelStatementFound && _endInheritsLocation.HasValue)
            {
                Context.OnError(_endInheritsLocation.Value, "The 'inherits' keyword is not allowed when a 'ModelType' keyword is used.");
            }
        }

        /// <summary>
        ///     Parses the modeltype statement.
        /// </summary>
        protected virtual bool ModelTypeDirective()
        {
            AssertDirective("ModelType");

            Span.CodeGenerator = SpanCodeGenerator.Null;
            Context.CurrentBlock.Type = BlockType.Directive;

            AcceptAndMoveNext();
            var endModelLocation = CurrentLocation;

            if (At(VBSymbolType.WhiteSpace))
            {
                Span.EditHandler.AcceptedCharacters = AcceptedCharacters.None;
            }

            AcceptWhile(VBSymbolType.WhiteSpace);
            Output(SpanKind.MetaCode);

            if (_modelStatementFound)
            {
                Context.OnError(endModelLocation, "Only one 'ModelType' statement is allowed in a file.");
            }
            _modelStatementFound = true;

            if (EndOfFile || At(VBSymbolType.WhiteSpace) || At(VBSymbolType.NewLine))
            {
                Context.OnError(endModelLocation, "The 'ModelType' keyword must be followed by a type name on the same line.");
            }

            // Just accept to a newline
            AcceptUntil(VBSymbolType.NewLine);
            if (!Context.DesignTimeMode)
            {
                // We want the newline to be treated as code, but it causes issues at design-time.
                Optional(VBSymbolType.NewLine);
            }

            var baseType = String.Concat(Span.Symbols.Select(s => s.Content)).Trim();
            Span.CodeGenerator = new SetModelTypeCodeGenerator(baseType, GenericTypeFormatString);

            CheckForInheritsAndModelStatements();
            Output(SpanKind.Code);
            return false;
        }

        #endregion
    }
}