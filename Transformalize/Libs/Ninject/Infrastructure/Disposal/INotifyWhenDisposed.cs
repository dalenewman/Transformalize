#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure.Disposal
{
    /// <summary>
    ///     An object that fires an event when it is disposed.
    /// </summary>
    public interface INotifyWhenDisposed : IDisposableObject
    {
        /// <summary>
        ///     Occurs when the object is disposed.
        /// </summary>
        event EventHandler Disposed;
    }
}