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
using System.Reflection;

namespace Transformalize.Libs.Ninject.Infrastructure.Language
{
    internal static class ExtensionsForICustomAttributeProvider
    {
        public static bool HasAttribute(this ICustomAttributeProvider member, Type type)
        {
            var memberInfo = member as MemberInfo;
            if (memberInfo != null)
            {
                return memberInfo.HasAttribute(type);
            }

            return member.IsDefined(type, true);
        }

        public static object[] GetCustomAttributesExtended(this ICustomAttributeProvider member, Type attributeType, bool inherit)
        {
            var memberInfo = member as MemberInfo;
            if (memberInfo != null)
            {
                return memberInfo.GetCustomAttributesExtended(attributeType, inherit);
            }

            return member.GetCustomAttributes(attributeType, inherit);
        }
    }
}