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

namespace Transformalize.Libs.NLog.Common
{
    /// <summary>
    ///     Asynchronous action.
    /// </summary>
    /// <param name="asyncContinuation">Continuation to be invoked at the end of action.</param>
    public delegate void AsynchronousAction(AsyncContinuation asyncContinuation);

    /// <summary>
    ///     Asynchronous action with one argument.
    /// </summary>
    /// <typeparam name="T">Type of the argument.</typeparam>
    /// <param name="argument">Argument to the action.</param>
    /// <param name="asyncContinuation">Continuation to be invoked at the end of action.</param>
    public delegate void AsynchronousAction<T>(T argument, AsyncContinuation asyncContinuation);
}