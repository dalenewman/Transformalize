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

using Transformalize.Libs.FileHelpers.DataLink.Storage;

namespace Transformalize.Libs.FileHelpers.Enums
{
    /// <summary>
    ///     Define the diferent Modes of Transaction that uses the <see cref="DatabaseStorage" />
    /// </summary>
    public enum TransactionMode
    {
        /// <summary>No transaction used.</summary>
        NoTransaction = 0,

        /// <summary>Default Transaction Mode.</summary>
        UseDefault,

        /// <summary>Chaos Level Transaction Mode.</summary>
        UseChaosLevel,

        /// <summary>ReadCommitted Transaction Mode.</summary>
        UseReadCommitted,

        /// <summary>ReadUnCommitted Transaction Mode.</summary>
        UseReadUnCommitted,

        /// <summary>Repeatable Transaction Mode.</summary>
        UseRepeatableRead,

        /// <summary>Serializable Transaction Mode.</summary>
        UseSerializable
    }
}