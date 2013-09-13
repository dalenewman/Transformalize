#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Internal;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Impersonates another user for the duration of the write.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/ImpersonatingWrapper_target">Documentation on NLog Wiki</seealso>
    [Target("ImpersonatingWrapper", IsWrapper = true)]
    public class ImpersonatingTargetWrapper : WrapperTargetBase
    {
        private IntPtr duplicateTokenHandle = IntPtr.Zero;
        private WindowsIdentity newIdentity;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImpersonatingTargetWrapper" /> class.
        /// </summary>
        public ImpersonatingTargetWrapper()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImpersonatingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        public ImpersonatingTargetWrapper(Target wrappedTarget)
        {
            Domain = ".";
            LogOnType = SecurityLogOnType.Interactive;
            LogOnProvider = LogOnProviderType.Default;
            ImpersonationLevel = SecurityImpersonationLevel.Impersonation;
            WrappedTarget = wrappedTarget;
        }

        /// <summary>
        ///     Gets or sets username to change context to.
        /// </summary>
        /// <docgen category='Impersonation Options' order='10' />
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the user account password.
        /// </summary>
        /// <docgen category='Impersonation Options' order='10' />
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets Windows domain name to change context to.
        /// </summary>
        /// <docgen category='Impersonation Options' order='10' />
        [DefaultValue(".")]
        public string Domain { get; set; }

        /// <summary>
        ///     Gets or sets the Logon Type.
        /// </summary>
        /// <docgen category='Impersonation Options' order='10' />
        public SecurityLogOnType LogOnType { get; set; }

        /// <summary>
        ///     Gets or sets the type of the logon provider.
        /// </summary>
        /// <docgen category='Impersonation Options' order='10' />
        public LogOnProviderType LogOnProvider { get; set; }

        /// <summary>
        ///     Gets or sets the required impersonation level.
        /// </summary>
        /// <docgen category='Impersonation Options' order='10' />
        public SecurityImpersonationLevel ImpersonationLevel { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to revert to the credentials of the process instead of impersonating another user.
        /// </summary>
        /// <docgen category='Impersonation Options' order='10' />
        [DefaultValue(false)]
        public bool RevertToSelf { get; set; }

        /// <summary>
        ///     Initializes the impersonation context.
        /// </summary>
        protected override void InitializeTarget()
        {
            if (!RevertToSelf)
            {
                newIdentity = CreateWindowsIdentity(out duplicateTokenHandle);
            }

            using (DoImpersonate())
            {
                base.InitializeTarget();
            }
        }

        /// <summary>
        ///     Closes the impersonation context.
        /// </summary>
        protected override void CloseTarget()
        {
            using (DoImpersonate())
            {
                base.CloseTarget();
            }

            if (duplicateTokenHandle != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(duplicateTokenHandle);
                duplicateTokenHandle = IntPtr.Zero;
            }

            if (newIdentity != null)
            {
                newIdentity.Dispose();
                newIdentity = null;
            }
        }

        /// <summary>
        ///     Changes the security context, forwards the call to the <see cref="WrapperTargetBase.WrappedTarget" />.Write()
        ///     and switches the context back to original.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            using (DoImpersonate())
            {
                WrappedTarget.WriteAsyncLogEvent(logEvent);
            }
        }

        /// <summary>
        ///     Changes the security context, forwards the call to the <see cref="WrapperTargetBase.WrappedTarget" />.Write()
        ///     and switches the context back to original.
        /// </summary>
        /// <param name="logEvents">Log events.</param>
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            using (DoImpersonate())
            {
                WrappedTarget.WriteAsyncLogEvents(logEvents);
            }
        }

        /// <summary>
        ///     Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            using (DoImpersonate())
            {
                WrappedTarget.Flush(asyncContinuation);
            }
        }

        private IDisposable DoImpersonate()
        {
            if (RevertToSelf)
            {
                return new ContextReverter(WindowsIdentity.Impersonate(IntPtr.Zero));
            }

            return new ContextReverter(newIdentity.Impersonate());
        }

        //
        // adapted from:
        // http://www.codeproject.com/csharp/cpimpersonation1.asp
        //
        private WindowsIdentity CreateWindowsIdentity(out IntPtr handle)
        {
            // initialize tokens
            IntPtr logonHandle;

            if (!NativeMethods.LogonUser(
                UserName,
                Domain,
                Password,
                (int) LogOnType,
                (int) LogOnProvider,
                out logonHandle))
            {
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            if (!NativeMethods.DuplicateToken(logonHandle, (int) ImpersonationLevel, out handle))
            {
                NativeMethods.CloseHandle(logonHandle);
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            NativeMethods.CloseHandle(logonHandle);

            // create new identity using new primary token)
            return new WindowsIdentity(handle);
        }

        /// <summary>
        ///     Helper class which reverts the given <see cref="WindowsImpersonationContext" />
        ///     to its original value as part of <see cref="IDisposable.Dispose" />.
        /// </summary>
        internal class ContextReverter : IDisposable
        {
            private readonly WindowsImpersonationContext wic;

            /// <summary>
            ///     Initializes a new instance of the <see cref="ContextReverter" /> class.
            /// </summary>
            /// <param name="windowsImpersonationContext">The windows impersonation context.</param>
            public ContextReverter(WindowsImpersonationContext windowsImpersonationContext)
            {
                wic = windowsImpersonationContext;
            }

            /// <summary>
            ///     Reverts the impersonation context.
            /// </summary>
            public void Dispose()
            {
                wic.Undo();
            }
        }
    }
}

#endif