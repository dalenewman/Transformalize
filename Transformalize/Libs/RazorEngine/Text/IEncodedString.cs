#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.RazorEngine.Text
{
    /// <summary>
    ///     Defines the required contract for implementing an encoded string.
    /// </summary>
    public interface IEncodedString
    {
        #region Methods

        /// <summary>
        ///     Gets the encoded string.
        /// </summary>
        /// <returns>The encoded string.</returns>
        string ToEncodedString();

        #endregion
    }
}