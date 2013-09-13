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
using System.Reflection;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Provides means to populate factories of named items (such as targets, layouts, layout renderers, etc.).
    /// </summary>
    internal interface IFactory
    {
        void Clear();

        void ScanAssembly(Assembly theAssembly, string prefix);

        void RegisterType(Type type, string itemNamePrefix);
    }
}