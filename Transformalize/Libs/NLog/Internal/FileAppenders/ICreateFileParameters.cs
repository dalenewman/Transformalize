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

using Transformalize.Libs.NLog.Targets;

namespace Transformalize.Libs.NLog.Internal.FileAppenders
{
    /// <summary>
    ///     Interface that provides parameters for create file function.
    /// </summary>
    internal interface ICreateFileParameters
    {
        int ConcurrentWriteAttemptDelay { get; }

        int ConcurrentWriteAttempts { get; }

        bool ConcurrentWrites { get; }

        bool CreateDirs { get; }

        bool EnableFileDelete { get; }

        int BufferSize { get; }

#if !NET_CF && !SILVERLIGHT
        Win32FileAttributes FileAttributes { get; }
#endif
    }
}