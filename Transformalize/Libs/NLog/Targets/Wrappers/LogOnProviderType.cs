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


#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Logon provider.
    /// </summary>
    public enum LogOnProviderType
    {
        /// <summary>
        ///     Use the standard logon provider for the system.
        /// </summary>
        /// <remarks>
        ///     The default security provider is negotiate, unless you pass NULL for the domain name and the user name
        ///     is not in UPN format. In this case, the default provider is NTLM.
        ///     NOTE: Windows 2000/NT:   The default security provider is NTLM.
        /// </remarks>
        Default = 0,
    }
}

#endif