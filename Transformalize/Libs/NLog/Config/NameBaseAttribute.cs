#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.NLog.LayoutRenderers;
using Transformalize.Libs.NLog.Layouts;
using Transformalize.Libs.NLog.Targets;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Attaches a simple name to an item (such as <see cref="Target" />,
    ///     <see cref="LayoutRenderer" />, <see cref="Layout" />, etc.).
    /// </summary>
    public abstract class NameBaseAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NameBaseAttribute" /> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        protected NameBaseAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets the name of the item.
        /// </summary>
        /// <value>The name of the item.</value>
        public string Name { get; private set; }
    }
}