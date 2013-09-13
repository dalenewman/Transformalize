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
using Transformalize.Libs.RazorEngine.Compilation.CSharp;
using Transformalize.Libs.RazorEngine.Compilation.VisualBasic;

namespace Transformalize.Libs.RazorEngine.Compilation
{
    /// <summary>
    ///     Provides a default implementation of a <see cref="ICompilerServiceFactory" />.
    /// </summary>
    public class DefaultCompilerServiceFactory : ICompilerServiceFactory
    {
        #region Methods

        /// <summary>
        ///     Creates a <see cref="ICompilerService" /> that supports the specified language.
        /// </summary>
        /// <param name="language">
        ///     The <see cref="Language" />.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="ICompilerService" />.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public ICompilerService CreateCompilerService(Language language)
        {
            switch (language)
            {
                case Language.CSharp:
                    return new CSharpDirectCompilerService();

                case Language.VisualBasic:
                    return new VBDirectCompilerService();

                default:
                    throw new ArgumentException("Unsupported language: " + language);
            }
        }

        #endregion
    }
}