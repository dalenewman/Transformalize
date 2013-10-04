#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Internal
{
#if SILVERLIGHT || NET_CF

namespace System.ComponentModel
{
    using System;

    /// <summary>
    /// Define Localizable attribute for platforms that don't have it.
    /// </summary>
    internal class LocalizableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizableAttribute"/> class.
        /// </summary>
        /// <param name="isLocalizable">Determines whether the target is localizable.</param>
        public LocalizableAttribute(bool isLocalizable)
        {
            IsLocalizable = isLocalizable;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the target is localizable.
        /// </summary>
        public bool IsLocalizable { get; set; }
    }
}

#endif
}