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

namespace Transformalize.Libs.RazorEngine.Compilation.VisualBasic
{
    /// <summary>
    ///     Provides a razor code language that supports the VB language.
    /// </summary>
    public class VBRazorCodeLanguage : System.Web.Razor.VBRazorCodeLanguage
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance
        /// </summary>
        /// <param name="strictMode">Flag to determine whether strict mode is enabled.</param>
        public VBRazorCodeLanguage(bool strictMode)
        {
            StrictMode = strictMode;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets whether strict mode is enabled.
        /// </summary>
        public bool StrictMode { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the code generator.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="rootNamespaceName">Name of the root namespace.</param>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="host">The host.</param>
        /// <returns>
        ///     An instance of <see cref="RazorCodeGenerator" />.
        /// </returns>
        public override RazorCodeGenerator CreateCodeGenerator(string className, string rootNamespaceName, string sourceFileName, System.Web.Razor.RazorEngineHost host)
        {
            return new VBRazorCodeGenerator(className, rootNamespaceName, sourceFileName, host, StrictMode);
        }

        #endregion
    }
}