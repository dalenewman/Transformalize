#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Layouts
{
    /// <summary>
    ///     Marks class as a layout renderer and assigns a format string to it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LayoutAttribute : NameBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LayoutAttribute" /> class.
        /// </summary>
        /// <param name="name">Layout name.</param>
        public LayoutAttribute(string name)
            : base(name)
        {
        }
    }
}