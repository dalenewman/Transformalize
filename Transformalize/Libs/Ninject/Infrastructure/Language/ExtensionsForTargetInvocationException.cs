#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using System.Reflection;

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure.Language
{
    internal static class ExtensionsForTargetInvocationException
    {
        public static void RethrowInnerException(this TargetInvocationException exception)
        {
            var innerException = exception.InnerException;

            var stackTraceField = typeof (Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            stackTraceField.SetValue(innerException, innerException.StackTrace);

            throw innerException;
        }
    }
}