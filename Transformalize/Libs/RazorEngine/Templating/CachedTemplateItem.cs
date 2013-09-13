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

using System;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Defines a cached template item.
    /// </summary>
    internal class CachedTemplateItem
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="CachedTemplateItem" />.
        /// </summary>
        /// <param name="cachedHashCode">The cached hash code.</param>
        /// <param name="templateType">The template type.</param>
        public CachedTemplateItem(int cachedHashCode, Type templateType)
        {
            CachedHashCode = cachedHashCode;
            TemplateType = templateType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the cached hash code of the template.
        /// </summary>
        public int CachedHashCode { get; private set; }

        /// <summary>
        ///     Gets the template type.
        /// </summary>
        public Type TemplateType { get; private set; }

        #endregion
    }
}