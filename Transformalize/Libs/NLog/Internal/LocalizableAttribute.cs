#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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