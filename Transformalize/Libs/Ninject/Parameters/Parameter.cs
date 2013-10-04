#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Planning.Targets;

#endregion

namespace Transformalize.Libs.Ninject.Parameters
{
    /// <summary>
    ///     Modifies an activation process in some way.
    /// </summary>
    public class Parameter : IParameter
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Parameter" /> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public Parameter(string name, object value, bool shouldInherit) : this(name, (ctx, target) => value, shouldInherit)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Parameter" /> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="valueCallback">The callback that will be triggered to get the parameter's value.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public Parameter(string name, Func<IContext, object> valueCallback, bool shouldInherit)
        {
            Ensure.ArgumentNotNullOrEmpty(name, "name");
            Ensure.ArgumentNotNull(valueCallback, "valueCallback");

            Name = name;
            ValueCallback = (ctx, target) => valueCallback(ctx);
            ShouldInherit = shouldInherit;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Parameter" /> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="valueCallback">The callback that will be triggered to get the parameter's value.</param>
        /// <param name="shouldInherit">Whether the parameter should be inherited into child requests.</param>
        public Parameter(string name, Func<IContext, ITarget, object> valueCallback, bool shouldInherit)
        {
            Ensure.ArgumentNotNullOrEmpty(name, "name");
            Ensure.ArgumentNotNull(valueCallback, "valueCallback");

            Name = name;
            ValueCallback = valueCallback;
            ShouldInherit = shouldInherit;
        }

        /// <summary>
        ///     Gets or sets the callback that will be triggered to get the parameter's value.
        /// </summary>
        public Func<IContext, ITarget, object> ValueCallback { get; internal set; }

        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the parameter should be inherited into child requests.
        /// </summary>
        public bool ShouldInherit { get; private set; }

        /// <summary>
        ///     Gets the value for the parameter within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>The value for the parameter.</returns>
        public object GetValue(IContext context, ITarget target)
        {
            Ensure.ArgumentNotNull(context, "context");
            return ValueCallback(context, target);
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     <c>True</c> if the objects are equal; otherwise <c>false</c>
        /// </returns>
        public bool Equals(IParameter other)
        {
            return other.GetType() == GetType() && other.Name.Equals(Name);
        }

        /// <summary>
        ///     Determines whether the object equals the specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns>
        ///     <c>True</c> if the objects are equal; otherwise <c>false</c>
        /// </returns>
        public override bool Equals(object obj)
        {
            var parameter = obj as IParameter;
            return parameter != null ? Equals(parameter) : base.Equals(obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the object.</returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ Name.GetHashCode();
        }
    }
}