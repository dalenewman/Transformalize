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

using Transformalize.Libs.Ninject.Activation;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Passed to ToConstructor to specify that a constructor value is Injected.
    /// </summary>
    public interface IConstructorArgumentSyntax : IFluentSyntax
    {
        /// <summary>
        ///     Gets the context.
        /// </summary>
        /// <value>The context.</value>
        IContext Context { get; }

        /// <summary>
        ///     Specifies that the argument is injected.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <returns>Not used. This interface has no implementation.</returns>
        T Inject<T>();
    }
}