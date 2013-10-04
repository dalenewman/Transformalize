#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.Ninject.Parameters
{
    /// <summary>
    ///     Overrides the injected value of a property.
    ///     Keeps a weak reference to the value.
    /// </summary>
    public class WeakPropertyValue : Parameter, IPropertyValue
    {
        private readonly WeakReference weakReference;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakPropertyValue" /> class.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value to inject into the property.</param>
        public WeakPropertyValue(string name, object value)
            : base(name, (object) null, false)
        {
            weakReference = new WeakReference(value);
            ValueCallback = (ctx, target) => weakReference.Target;
        }
    }
}