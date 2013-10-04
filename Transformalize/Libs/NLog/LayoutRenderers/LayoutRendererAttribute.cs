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
    ///     Marks class as a layout renderer and assigns a format string to it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LayoutRendererAttribute : NameBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LayoutRendererAttribute" /> class.
        /// </summary>
        /// <param name="name">Name of the layout renderer.</param>
        public LayoutRendererAttribute(string name)
            : base(name)
        {
        }
    }
}