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
    ///     A service that is started when activated, and stopped when deactivated.
    /// </summary>
    public interface IStartable
    {
        /// <summary>
        ///     Starts this instance. Called during activation.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops this instance. Called during deactivation.
        /// </summary>
        void Stop();
    }
}