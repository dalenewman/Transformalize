#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Used to mark configurable parameters which are arrays.
    ///     Specifies the mapping between XML elements and .NET types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ArrayParameterAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ArrayParameterAttribute" /> class.
        /// </summary>
        /// <param name="itemType">The type of the array item.</param>
        /// <param name="elementName">The XML element name that represents the item.</param>
        public ArrayParameterAttribute(Type itemType, string elementName)
        {
            ItemType = itemType;
            ElementName = elementName;
        }

        /// <summary>
        ///     Gets the .NET type of the array item.
        /// </summary>
        public Type ItemType { get; private set; }

        /// <summary>
        ///     Gets the XML element name.
        /// </summary>
        public string ElementName { get; private set; }
    }
}