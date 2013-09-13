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

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Allows base templates to define require template imports when
    ///     generating templates.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class RequireNamespacesAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="RequireNamespacesAttribute" />.
        /// </summary>
        /// <param name="namespaces">The set of required namespace imports.</param>
        public RequireNamespacesAttribute(params string[] namespaces)
        {
            if (namespaces == null)
                throw new ArgumentNullException("namespaces");

            var set = new HashSet<string>();
            foreach (var ns in namespaces)
                set.Add(ns);

            Namespaces = set;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the set of required namespace imports.
        /// </summary>
        public IEnumerable<string> Namespaces { get; private set; }

        #endregion
    }
}