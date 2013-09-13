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
using System.Web.Razor;
using System.Web.Razor.Parser;

namespace Transformalize.Libs.RazorEngine.Compilation
{
    /// <summary>
    ///     Defines the custom razor engine host.
    /// </summary>
    public class RazorEngineHost : System.Web.Razor.RazorEngineHost
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="RazorEngineHost" />.
        /// </summary>
        /// <param name="language">The code language.</param>
        /// <param name="markupParserFactory">The markup parser factory delegate.</param>
        public RazorEngineHost(RazorCodeLanguage language, Func<ParserBase> markupParserFactory)
            : base(language, markupParserFactory)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the default template type.
        /// </summary>
        public Type DefaultBaseTemplateType { get; set; }

        /// <summary>
        ///     Gets or sets the default model type.
        /// </summary>
        public Type DefaultModelType { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Decorates the code parser.
        /// </summary>
        /// <param name="incomingCodeParser">The code parser.</param>
        /// <returns>The decorated parser.</returns>
        public override ParserBase DecorateCodeParser(ParserBase incomingCodeParser)
        {
            if (incomingCodeParser is CSharpCodeParser)
                return new CSharp.CSharpCodeParser();

            if (incomingCodeParser is VBCodeParser)
                return new VisualBasic.VBCodeParser();

            return base.DecorateCodeParser(incomingCodeParser);
        }

        #endregion
    }
}