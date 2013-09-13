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

using Transformalize.Libs.Ninject.Infrastructure.Disposal;

#endregion

namespace Transformalize.Libs.Ninject.Components
{
    /// <summary>
    ///     A component that contributes to the internals of Ninject.
    /// </summary>
    public abstract class NinjectComponent : DisposableObject, INinjectComponent
    {
        /// <summary>
        ///     Gets or sets the settings.
        /// </summary>
        public INinjectSettings Settings { get; set; }
    }
}