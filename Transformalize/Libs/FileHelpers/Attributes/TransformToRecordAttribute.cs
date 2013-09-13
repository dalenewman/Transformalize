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

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>With this attribute you can mark a method in the RecordClass that is the responsable of convert it to the specified.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TransformToRecordAttribute : Attribute
    {
        internal Type TargetType;

        /// <summary>With this attribute you can mark a method in the RecordClass that is the responsable of convert it to the specified.</summary>
        /// <param name="targetType">The target of the convertion.</param>
        public TransformToRecordAttribute(Type targetType)
        {
            //throw new NotImplementedException("This feature is not ready yet. In the next release maybe work =)");
            TargetType = targetType;
        }
    }
}