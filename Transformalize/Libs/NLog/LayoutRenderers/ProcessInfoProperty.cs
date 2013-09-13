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


#if !NET_CF && !MONO && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Property of System.Diagnostics.Process to retrieve.
    /// </summary>
    public enum ProcessInfoProperty
    {
        /// <summary>
        ///     Base Priority.
        /// </summary>
        BasePriority,

        /// <summary>
        ///     Exit Code.
        /// </summary>
        ExitCode,

        /// <summary>
        ///     Exit Time.
        /// </summary>
        ExitTime,

        /// <summary>
        ///     Process Handle.
        /// </summary>
        Handle,

        /// <summary>
        ///     Handle Count.
        /// </summary>
        HandleCount,

        /// <summary>
        ///     Whether process has exited.
        /// </summary>
        HasExited,

        /// <summary>
        ///     Process ID.
        /// </summary>
        Id,

        /// <summary>
        ///     Machine name.
        /// </summary>
        MachineName,

        /// <summary>
        ///     Handle of the main window.
        /// </summary>
        MainWindowHandle,

        /// <summary>
        ///     Title of the main window.
        /// </summary>
        MainWindowTitle,

        /// <summary>
        ///     Maximum Working Set.
        /// </summary>
        MaxWorkingSet,

        /// <summary>
        ///     Minimum Working Set.
        /// </summary>
        MinWorkingSet,

        /// <summary>
        ///     Non-paged System Memory Size.
        /// </summary>
        NonPagedSystemMemorySize,

        /// <summary>
        ///     Non-paged System Memory Size (64-bit).
        /// </summary>
        NonPagedSystemMemorySize64,

        /// <summary>
        ///     Paged Memory Size.
        /// </summary>
        PagedMemorySize,

        /// <summary>
        ///     Paged Memory Size (64-bit)..
        /// </summary>
        PagedMemorySize64,

        /// <summary>
        ///     Paged System Memory Size.
        /// </summary>
        PagedSystemMemorySize,

        /// <summary>
        ///     Paged System Memory Size (64-bit).
        /// </summary>
        PagedSystemMemorySize64,

        /// <summary>
        ///     Peak Paged Memory Size.
        /// </summary>
        PeakPagedMemorySize,

        /// <summary>
        ///     Peak Paged Memory Size (64-bit).
        /// </summary>
        PeakPagedMemorySize64,

        /// <summary>
        ///     Peak Vitual Memory Size.
        /// </summary>
        PeakVirtualMemorySize,

        /// <summary>
        ///     Peak Virtual Memory Size (64-bit)..
        /// </summary>
        PeakVirtualMemorySize64,

        /// <summary>
        ///     Peak Working Set Size.
        /// </summary>
        PeakWorkingSet,

        /// <summary>
        ///     Peak Working Set Size (64-bit).
        /// </summary>
        PeakWorkingSet64,

        /// <summary>
        ///     Whether priority boost is enabled.
        /// </summary>
        PriorityBoostEnabled,

        /// <summary>
        ///     Priority Class.
        /// </summary>
        PriorityClass,

        /// <summary>
        ///     Private Memory Size.
        /// </summary>
        PrivateMemorySize,

        /// <summary>
        ///     Private Memory Size (64-bit).
        /// </summary>
        PrivateMemorySize64,

        /// <summary>
        ///     Privileged Processor Time.
        /// </summary>
        PrivilegedProcessorTime,

        /// <summary>
        ///     Process Name.
        /// </summary>
        ProcessName,

        /// <summary>
        ///     Whether process is responding.
        /// </summary>
        Responding,

        /// <summary>
        ///     Session ID.
        /// </summary>
        SessionId,

        /// <summary>
        ///     Process Start Time.
        /// </summary>
        StartTime,

        /// <summary>
        ///     Total Processor Time.
        /// </summary>
        TotalProcessorTime,

        /// <summary>
        ///     User Processor Time.
        /// </summary>
        UserProcessorTime,

        /// <summary>
        ///     Virtual Memory Size.
        /// </summary>
        VirtualMemorySize,

        /// <summary>
        ///     Virtual Memory Size (64-bit).
        /// </summary>
        VirtualMemorySize64,

        /// <summary>
        ///     Working Set Size.
        /// </summary>
        WorkingSet,

        /// <summary>
        ///     Working Set Size (64-bit).
        /// </summary>
        WorkingSet64,
    }
}

#endif