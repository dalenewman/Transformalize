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
using System.Collections.Generic;

namespace Transformalize.Libs.RazorEngine.Compilation
{
    /// <summary>
    ///     Defines a type context that describes a template to compile.
    /// </summary>
    public class TypeContext : MarshalByRefObject
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="TypeContext" />.
        /// </summary>
        internal TypeContext()
        {
            ClassName = CompilerServicesUtility.GenerateClassName();
            Namespaces = new HashSet<string>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the class name.
        /// </summary>
        public string ClassName { get; private set; }

        /// <summary>
        ///     Gets or sets the model type.
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        ///     Gets the set of namespace imports.
        /// </summary>
        public ISet<string> Namespaces { get; private set; }

        /// <summary>
        ///     Gets or sets the template content.
        /// </summary>
        public string TemplateContent { get; set; }

        /// <summary>
        ///     Gets or sets the base template type.
        /// </summary>
        public Type TemplateType { get; set; }

        #endregion
    }
}