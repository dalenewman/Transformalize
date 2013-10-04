#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using Transformalize.Libs.Ninject.Activation;

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure
{
    /// <summary>
    ///     Scope callbacks for standard scopes.
    /// </summary>
    public class StandardScopeCallbacks
    {
        /// <summary>
        ///     Gets the callback for transient scope.
        /// </summary>
        public static readonly Func<IContext, object> Transient = ctx => null;

        /// <summary>
        ///     Gets the callback for singleton scope.
        /// </summary>
        public static readonly Func<IContext, object> Singleton = ctx => ctx.Kernel;

        /// <summary>
        ///     Gets the callback for thread scope.
        /// </summary>
        public static readonly Func<IContext, object> Thread = ctx => System.Threading.Thread.CurrentThread;
    }
}