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

using System.Reflection;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Represents a factory of named items (such as targets, layouts, layout renderers, etc.).
    /// </summary>
    /// <typeparam name="TInstanceType">Base type for each item instance.</typeparam>
    /// <typeparam name="TDefinitionType">
    ///     Item definition type (typically <see cref="System.Type" /> or <see cref="MethodInfo" />).
    /// </typeparam>
    public interface INamedItemFactory<TInstanceType, TDefinitionType>
        where TInstanceType : class
    {
        /// <summary>
        ///     Registers new item definition.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="itemDefinition">Item definition.</param>
        void RegisterDefinition(string itemName, TDefinitionType itemDefinition);

        /// <summary>
        ///     Tries to get registed item definition.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="result">Reference to a variable which will store the item definition.</param>
        /// <returns>Item definition.</returns>
        bool TryGetDefinition(string itemName, out TDefinitionType result);

        /// <summary>
        ///     Creates item instance.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns>Newly created item instance.</returns>
        TInstanceType CreateInstance(string itemName);

        /// <summary>
        ///     Tries to create an item instance.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="result">The result.</param>
        /// <returns>True if instance was created successfully, false otherwise.</returns>
        bool TryCreateInstance(string itemName, out TInstanceType result);
    }
}