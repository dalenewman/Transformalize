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

using System.Diagnostics.CodeAnalysis;

namespace Transformalize.Libs.Ninject.Syntax
{
    /// <summary>
    ///     Used to set the condition, scope, name, or add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
    public interface IBindingWhenInNamedWithOrOnSyntax<T> : IBindingWhenSyntax<T>,
                                                            IBindingInSyntax<T>,
                                                            IBindingNamedSyntax<T>,
                                                            IBindingWithSyntax<T>,
                                                            IBindingOnSyntax<T>
    {
    }
}