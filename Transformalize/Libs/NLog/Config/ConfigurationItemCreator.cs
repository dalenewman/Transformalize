#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Constructs a new instance the configuration item (target, layout, layout renderer, etc.) given its type.
    /// </summary>
    /// <param name="itemType">Type of the item.</param>
    /// <returns>Created object of the specified type.</returns>
    public delegate object ConfigurationItemCreator(Type itemType);
}