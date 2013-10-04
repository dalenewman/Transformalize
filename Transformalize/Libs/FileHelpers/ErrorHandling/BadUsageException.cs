#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.FileHelpers.ErrorHandling
{
    /// <summary>Indicates the wrong usage of the library.</summary>
    public class BadUsageException : FileHelpersException
    {
        /// <summary>Creates an instance of an BadUsageException.</summary>
        /// <param name="message">The exception Message</param>
        protected internal BadUsageException(string message) : base(message)
        {
        }

//		/// <summary>Creates an instance of an BadUsageException.</summary>
//		/// <param name="message">The exception Message</param>
//		/// <param name="innerEx">The inner exception.</param>
//		protected internal BadUsageException(string message, Exception innerEx) : base(message, innerEx)
//		{
//		}
    }
}