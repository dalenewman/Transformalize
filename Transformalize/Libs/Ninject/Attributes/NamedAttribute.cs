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

#region Using Directives

using Transformalize.Libs.Ninject.Infrastructure;
using Transformalize.Libs.Ninject.Planning.Bindings;

#endregion

namespace Transformalize.Libs.Ninject.Attributes
{
    /// <summary>
    ///     Indicates that the decorated member should only be injected using binding(s) registered
    ///     with the specified name.
    /// </summary>
    public class NamedAttribute : ConstraintAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NamedAttribute" /> class.
        /// </summary>
        /// <param name="name">The name of the binding(s) to use.</param>
        public NamedAttribute(string name)
        {
            Ensure.ArgumentNotNullOrEmpty(name, "name");
            Name = name;
        }

        /// <summary>
        ///     Gets the binding name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Determines whether the specified binding metadata matches the constraint.
        /// </summary>
        /// <param name="metadata">The metadata in question.</param>
        /// <returns>
        ///     <c>True</c> if the metadata matches; otherwise <c>false</c>.
        /// </returns>
        public override bool Matches(IBindingMetadata metadata)
        {
            Ensure.ArgumentNotNull(metadata, "metadata");
            return metadata.Name == Name;
        }
    }
}