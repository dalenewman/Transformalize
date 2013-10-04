#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

#endregion

namespace Transformalize.Libs.Ninject
{
    /// <summary>
    ///     A service that requires initialization after it is activated.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        ///     Initializes the instance. Called during activation.
        /// </summary>
        void Initialize();
    }
}