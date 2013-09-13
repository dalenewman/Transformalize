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
    ///     Additional information available about a binding, which can be used in constraints
    ///     to select bindings to use in activation.
    /// </summary>
    public interface IBindingMetadata
    {
        /// <summary>
        ///     Gets or sets the binding's name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Determines whether a piece of metadata with the specified key has been defined.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <returns>
        ///     <c>True</c> if such a piece of metadata exists; otherwise, <c>false</c>.
        /// </returns>
        bool Has(string key);

        /// <summary>
        ///     Gets the value of metadata defined with the specified key, cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of value to expect.</typeparam>
        /// <param name="key">The metadata key.</param>
        /// <returns>The metadata value.</returns>
        T Get<T>(string key);

        /// <summary>
        ///     Gets the value of metadata defined with the specified key.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="defaultValue">The value to return if the binding has no metadata set with the specified key.</param>
        /// <returns>The metadata value, or the default value if none was set.</returns>
        T Get<T>(string key, T defaultValue);

        /// <summary>
        ///     Sets the value of a piece of metadata.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        void Set(string key, object value);
    }
}