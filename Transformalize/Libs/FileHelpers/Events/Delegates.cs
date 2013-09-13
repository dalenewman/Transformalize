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

using Transformalize.Libs.FileHelpers.Engines;

namespace Transformalize.Libs.FileHelpers.Events
{
    // ----  Read Operations  ----

    /// <summary>
    ///     Called in read operations just before the record string is translated to a record.
    /// </summary>
    /// <param name="engine">The engine that generates the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void BeforeReadRecordHandler<T>(EngineBase engine, BeforeReadRecordEventArgs<T> e);

    /// <summary>
    ///     Called in read operations just after the record was created from a record string.
    /// </summary>
    /// <param name="engine">The engine that generates the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void AfterReadRecordHandler<T>(EngineBase engine, AfterReadRecordEventArgs<T> e);


    // ----  Write Operations  ----

    /// <summary>
    ///     Called in write operations just before the record is converted to a string to write it.
    /// </summary>
    /// <param name="engine">The engine that generates the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void BeforeWriteRecordHandler<T>(EngineBase engine, BeforeWriteRecordEventArgs<T> e);

    /// <summary>
    ///     Called in write operations just after the record was converted to a string.
    /// </summary>
    /// <param name="engine">The engine that generates the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void AfterWriteRecordHandler<T>(EngineBase engine, AfterWriteRecordEventArgs<T> e);


    /// <summary>
    ///     Called in read operations just before the record string is translated to a record.
    /// </summary>
    /// <param name="engine">The engine that generates the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void BeforeReadRecordHandler(EngineBase engine, BeforeReadRecordEventArgs e);

    /// <summary>
    ///     Called in read operations just after the record was created from a record string.
    /// </summary>
    /// <param name="engine">The engine that generates the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void AfterReadRecordHandler(EngineBase engine, AfterReadRecordEventArgs e);


    // ----  Write Operations  ----

    /// <summary>
    ///     Called in write operations just before the record is converted to a string to write it.
    /// </summary>
    /// <param name="engine">The engine that generates the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void BeforeWriteRecordHandler(EngineBase engine, BeforeWriteRecordEventArgs e);

    /// <summary>
    ///     Called in write operations just after the record was converted to a string.
    /// </summary>
    /// <param name="engine">The engine that generates the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void AfterWriteRecordHandler(EngineBase engine, AfterWriteRecordEventArgs e);
}