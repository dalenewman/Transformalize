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
using System.Diagnostics.CodeAnalysis;
using System.Web.Razor.Parser;
using Microsoft.VisualBasic;

namespace Transformalize.Libs.RazorEngine.Compilation.VisualBasic
{
    /// <summary>
    ///     Defines a direct compiler service for the VB syntax.
    /// </summary>
    public class VBDirectCompilerService : DirectCompilerServiceBase
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="VBDirectCompilerService" />.
        /// </summary>
        /// <param name="strictMode">Specifies whether the strict mode parsing is enabled.</param>
        /// <param name="markupParserFactory">The markup parser to use.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in base class: DirectCompilerServiceBase")]
        public VBDirectCompilerService(bool strictMode = true, Func<ParserBase> markupParserFactory = null)
            : base(
                new VBRazorCodeLanguage(strictMode),
                new VBCodeProvider(),
                markupParserFactory)
        {
        }

        #endregion
    }
}