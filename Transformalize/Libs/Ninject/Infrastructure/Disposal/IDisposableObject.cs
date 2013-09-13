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

using System;

#endregion

namespace Transformalize.Libs.Ninject.Infrastructure.Disposal
{
    /// <summary>
    ///     An object that can report whether or not it is disposed.
    /// </summary>
    public interface IDisposableObject : IDisposable
    {
        /// <summary>
        ///     Gets a value indicating whether this instance is disposed.
        /// </summary>
        bool IsDisposed { get; }
    }
}