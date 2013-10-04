#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;

#endregion

namespace Transformalize.Libs.Ninject.Activation
{
    /// <summary>
    ///     Holds an instance during activation or after it has been cached.
    /// </summary>
    public class InstanceReference
    {
        /// <summary>
        ///     Gets or sets the instance.
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        ///     Returns a value indicating whether the instance is of the specified type.
        /// </summary>
        /// <typeparam name="T">The type in question.</typeparam>
        /// <returns>
        ///     <see langword="True" /> if the instance is of the specified type, otherwise <see langword="false" />.
        /// </returns>
        public bool Is<T>()
        {
            return Instance is T;
        }

        /// <summary>
        ///     Returns the instance as the specified type.
        /// </summary>
        /// <typeparam name="T">The requested type.</typeparam>
        /// <returns>The instance.</returns>
        public T As<T>()
        {
            return (T) Instance;
        }

        /// <summary>
        ///     Executes the specified action if the instance if of the specified type.
        /// </summary>
        /// <typeparam name="T">The type in question.</typeparam>
        /// <param name="action">The action to execute.</param>
        public void IfInstanceIs<T>(Action<T> action)
        {
            if (Instance is T)
                action((T) Instance);
        }
    }
}