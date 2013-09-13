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

#endregion

namespace Transformalize.Libs.Ninject.Planning.Bindings
{
    /// <summary>
    ///     Describes the target of a binding.
    /// </summary>
    public enum BindingTarget
    {
        /// <summary>
        ///     Indicates that the binding is from a type to itself.
        /// </summary>
        Self,

        /// <summary>
        ///     Indicates that the binding is from one type to another.
        /// </summary>
        Type,

        /// <summary>
        ///     Indicates that the binding is from a type to a provider.
        /// </summary>
        Provider,

        /// <summary>
        ///     Indicates that the binding is from a type to a callback method.
        /// </summary>
        Method,

        /// <summary>
        ///     Indicates that the binding is from a type to a constant value.
        /// </summary>
        Constant
    }
}