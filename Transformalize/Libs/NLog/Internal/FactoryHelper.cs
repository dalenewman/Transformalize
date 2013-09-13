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

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Object construction helper.
    /// </summary>
    internal class FactoryHelper
    {
        private static readonly Type[] emptyTypes = new Type[0];
        private static readonly object[] emptyParams = new object[0];

        private FactoryHelper()
        {
        }

        internal static object CreateInstance(Type t)
        {
            var constructor = t.GetConstructor(emptyTypes);
            if (constructor != null)
            {
                return constructor.Invoke(emptyParams);
            }
            else
            {
                throw new NLogConfigurationException("Cannot access the constructor of type: " + t.FullName + ". Is the required permission granted?");
            }
        }
    }
}