#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Filters
{
    /// <summary>
    ///     Marks class as a layout renderer and assigns a name to it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FilterAttribute : NameBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FilterAttribute" /> class.
        /// </summary>
        /// <param name="name">Name of the filter.</param>
        public FilterAttribute(string name)
            : base(name)
        {
        }
    }
}