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
using System.Diagnostics.Contracts;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Defines contextual information for a template instance.
    /// </summary>
    public class InstanceContext
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="InstanceContext" />.
        /// </summary>
        /// <param name="loader">The type loader.</param>
        /// <param name="templateType">The template type.</param>
        internal InstanceContext(TypeLoader loader, Type templateType)
        {
            Contract.Requires(loader != null);
            Contract.Requires(templateType != null);

            Loader = loader;
            TemplateType = templateType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the type loader.
        /// </summary>
        public TypeLoader Loader { get; private set; }

        /// <summary>
        ///     Gets the template type.
        /// </summary>
        public Type TemplateType { get; private set; }

        #endregion
    }
}