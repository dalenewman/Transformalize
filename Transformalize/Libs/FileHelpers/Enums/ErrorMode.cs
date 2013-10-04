#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.FileHelpers.Engines;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.Enums
{
    /// <summary>
    ///     Indicates the behavior when the <see cref="FileHelperEngine" /> class found an error.
    /// </summary>
    public enum ErrorMode
    {
        /// <summary>Default value, this simple Rethrow the original exception.</summary>
        ThrowException = 0,

        /// <summary>
        ///     Add an <see cref="ErrorInfo" /> to the array of <see cref="ErrorManager.Errors" />.
        /// </summary>
        SaveAndContinue,

        /// <summary>Simply ignores the exception an continue.</summary>
        IgnoreAndContinue
    }
}