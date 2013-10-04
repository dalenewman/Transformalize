#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Common
{
    /// <summary>
    ///     Asynchronous action.
    /// </summary>
    /// <param name="asyncContinuation">Continuation to be invoked at the end of action.</param>
    public delegate void AsynchronousAction(AsyncContinuation asyncContinuation);

    /// <summary>
    ///     Asynchronous action with one argument.
    /// </summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="argument">Argument to the action.</param>
    /// <param name="asyncContinuation">Continuation to be invoked at the end of action.</param>
    public delegate void AsynchronousAction<T>(T argument, AsyncContinuation asyncContinuation);
}