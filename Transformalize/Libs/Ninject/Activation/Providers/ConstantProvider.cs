#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

#endregion

namespace Transformalize.Libs.Ninject.Activation.Providers
{
    /// <summary>
    ///     A provider that always returns the same constant value.
    /// </summary>
    /// <typeparam name="T">The type of value that is returned.</typeparam>
    public class ConstantProvider<T> : Provider<T>
    {
        /// <summary>
        ///     Initializes a new instance of the ConstantProvider&lt;T&gt; class.
        /// </summary>
        /// <param name="value">The value that the provider should return.</param>
        public ConstantProvider(T value)
        {
            Value = value;
        }

        /// <summary>
        ///     Gets the value that the provider will return.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        ///     Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The constant value this provider returns.</returns>
        protected override T CreateInstance(IContext context)
        {
            return Value;
        }
    }
}