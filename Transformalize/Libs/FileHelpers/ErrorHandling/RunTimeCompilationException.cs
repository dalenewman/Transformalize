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