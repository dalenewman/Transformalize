#region License
// /*
// See license included in this library folder.
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