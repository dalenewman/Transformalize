#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Defines the required contract for implementing a template with a model.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public interface ITemplate<T> : ITemplate
    {
        #region Properties

        /// <summary>
        ///     Gets the or sets the model.
        /// </summary>
        T Model { get; set; }

        #endregion
    }
}