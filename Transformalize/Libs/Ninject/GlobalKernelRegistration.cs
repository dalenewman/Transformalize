#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Transformalize.Libs.Ninject
{
    /// <summary>
    ///     Allows to register kernel globally to perform some tasks on all kernels.
    ///     The registration is done by loading the GlobalKernelRegistrationModule to the kernel.
    /// </summary>
    public abstract class GlobalKernelRegistration
    {
        private static readonly ReaderWriterLock kernelRegistrationsLock = new ReaderWriterLock();
        private static readonly IDictionary<Type, Registration> kernelRegistrations = new Dictionary<Type, Registration>();

        internal static void RegisterKernelForType(IKernel kernel, Type type)
        {
            var registration = GetRegistrationForType(type);
            registration.KernelLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                registration.Kernels.Add(new WeakReference(kernel));
            }
            finally
            {
                registration.KernelLock.ReleaseWriterLock();
            }
        }

        internal static void UnregisterKernelForType(IKernel kernel, Type type)
        {
            var registration = GetRegistrationForType(type);
            RemoveKernels(registration, registration.Kernels.Where(reference => reference.Target == kernel || !reference.IsAlive));
        }

        /// <summary>
        ///     Performs an action on all registered kernels.
        /// </summary>
        /// <param name="action">The action.</param>
        protected void MapKernels(Action<IKernel> action)
        {
            var requiresCleanup = false;
            var registration = GetRegistrationForType(GetType());
            registration.KernelLock.AcquireReaderLock(Timeout.Infinite);

            try
            {
                foreach (var weakReference in registration.Kernels)
                {
                    var kernel = weakReference.Target as IKernel;
                    if (kernel != null)
                    {
                        action(kernel);
                    }
                    else
                    {
                        requiresCleanup = true;
                    }
                }
            }
            finally
            {
                registration.KernelLock.ReleaseReaderLock();
            }

            if (requiresCleanup)
            {
                RemoveKernels(registration, registration.Kernels.Where(reference => !reference.IsAlive));
            }
        }

        private static void RemoveKernels(Registration registration, IEnumerable<WeakReference> references)
        {
            registration.KernelLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                foreach (var reference in references.ToArray())
                {
                    registration.Kernels.Remove(reference);
                }
            }
            finally
            {
                registration.KernelLock.ReleaseWriterLock();
            }
        }

        private static Registration GetRegistrationForType(Type type)
        {
            kernelRegistrationsLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                Registration registration;
                if (kernelRegistrations.TryGetValue(type, out registration))
                {
                    return registration;
                }

                return CreateNewRegistration(type);
            }
            finally
            {
                kernelRegistrationsLock.ReleaseReaderLock();
            }
        }

        private static Registration CreateNewRegistration(Type type)
        {
            var lockCookie = kernelRegistrationsLock.UpgradeToWriterLock(Timeout.Infinite);
            try
            {
                Registration registration;
                if (kernelRegistrations.TryGetValue(type, out registration))
                {
                    return registration;
                }

                registration = new Registration();
                kernelRegistrations.Add(type, registration);
                return registration;
            }
            finally
            {
                kernelRegistrationsLock.DowngradeFromWriterLock(ref lockCookie);
            }
        }

        private class Registration
        {
            public Registration()
            {
                KernelLock = new ReaderWriterLock();
                Kernels = new List<WeakReference>();
            }

            public ReaderWriterLock KernelLock { get; private set; }
            public IList<WeakReference> Kernels { get; private set; }
        }
    }
}