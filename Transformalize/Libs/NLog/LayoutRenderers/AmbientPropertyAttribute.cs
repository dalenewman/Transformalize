#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Designates a property of the class as an ambient property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class AmbientPropertyAttribute : NameBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AmbientPropertyAttribute" /> class.
        /// </summary>
        /// <param name="name">Ambient property name.</param>
        public AmbientPropertyAttribute(string name)
            : base(name)
        {
        }
    }
}