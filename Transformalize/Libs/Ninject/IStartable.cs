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

namespace Transformalize.Libs.Ninject
{
    /// <summary>
    ///     A service that is started when activated, and stopped when deactivated.
    /// </summary>
    public interface IStartable
    {
        /// <summary>
        ///     Starts this instance. Called during activation.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops this instance. Called during deactivation.
        /// </summary>
        void Stop();
    }
}