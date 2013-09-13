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

using System.CodeDom.Compiler;

#region "  © Copyright 2005-07 to Marcos Meli - http://www.marcosmeli.com.ar" 

// Errors, suggestions, contributions, send a mail to: marcos@filehelpers.com.

#endregion

namespace Transformalize.Libs.FileHelpers.ErrorHandling
{
    /// <summary>
    ///     Exception with error information of the run time compilation.
    /// </summary>
    public sealed class RunTimeCompilationException : FileHelpersException
    {
        private readonly CompilerErrorCollection mCompilerErrors;
        private readonly string mSourceCode;

        internal RunTimeCompilationException(string message, string sourceCode, CompilerErrorCollection errors) : base(message)
        {
            mSourceCode = sourceCode;
            mCompilerErrors = errors;
        }

        /// <summary>
        ///     The source code that generates the Exception
        /// </summary>
        public string SourceCode
        {
            get { return mSourceCode; }
        }

        /// <summary>
        ///     The errors returned from the compiler.
        /// </summary>
        public CompilerErrorCollection CompilerErrors
        {
            get { return mCompilerErrors; }
        }
    }
}