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

namespace Transformalize.Libs.NLog.Internal.NetworkSenders
{
    /// <summary>
    ///     Creates instances of <see cref="NetworkSender" /> objects for given URLs.
    /// </summary>
    internal interface INetworkSenderFactory
    {
        /// <summary>
        ///     Creates a new instance of the network sender based on a network URL.
        /// </summary>
        /// <param name="url">
        ///     URL that determines the network sender to be created.
        /// </param>
        /// <returns>
        ///     A newly created network sender.
        /// </returns>
        NetworkSender Create(string url);
    }
}