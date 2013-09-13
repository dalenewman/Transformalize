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
using System.Diagnostics;
using System.Dynamic;

namespace Transformalize.Libs.RazorEngine.Compilation
{
    /// <summary>
    ///     Defines a dynamic object.
    /// </summary>
    internal class RazorDynamicObject : DynamicObject
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the model.
        /// </summary>
        public object Model { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the value of the specified member.
        /// </summary>
        /// <param name="binder">The current binder.</param>
        /// <param name="result">The member result.</param>
        /// <returns>True.</returns>
        [DebuggerStepThrough]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null)
                throw new ArgumentNullException("binder");

            var dynamicObject = Model as RazorDynamicObject;
            if (dynamicObject != null)
                return dynamicObject.TryGetMember(binder, out result);

            var modelType = Model.GetType();
            var prop = modelType.GetProperty(binder.Name);
            if (prop == null)
            {
                result = null;
                return false;
            }

            var value = prop.GetValue(Model, null);
            if (value == null)
            {
                result = value;
                return true;
            }

            var valueType = value.GetType();

            result = (CompilerServicesUtility.IsAnonymousType(valueType))
                         ? new RazorDynamicObject
                               {
                                   Model = value
                               }
                         : value;
            return true;
        }

        #endregion
    }
}