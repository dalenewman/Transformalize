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

using System.Web.Razor.Parser.SyntaxTree;
using Transformalize.Libs.RazorEngine.Templating;

namespace Transformalize.Libs.RazorEngine.Compilation.VisualBasic
{
    /// <summary>
    ///     Defines a code generator that supports VB syntax.
    /// </summary>
    public class VBRazorCodeGenerator : System.Web.Razor.Generator.VBRazorCodeGenerator
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="VBRazorCodeGenerator" /> class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="rootNamespaceName">Name of the root namespace.</param>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="host">The host.</param>
        /// <param name="strictMode">Flag to specify that this generator is running in struct mode.</param>
        public VBRazorCodeGenerator(string className, string rootNamespaceName, string sourceFileName, System.Web.Razor.RazorEngineHost host, bool strictMode)
            : base(className, rootNamespaceName, sourceFileName, host)
        {
            StrictMode = strictMode;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets whether the code generator is running in strict mode.
        /// </summary>
        public bool StrictMode { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Visits an error generated through parsing.
        /// </summary>
        /// <param name="err">The error that was generated.</param>
        public override void VisitError(RazorError err)
        {
            if (StrictMode)
                throw new TemplateParsingException(err);
        }

        #endregion
    }
}