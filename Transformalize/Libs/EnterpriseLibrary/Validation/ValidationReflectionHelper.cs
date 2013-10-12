//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Validation Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Helper for reflection access.
    /// </summary>
    public static class ValidationReflectionHelper
    {
        internal static PropertyInfo GetProperty(Type type, string propertyName, bool throwIfInvalid)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (!IsValidProperty(propertyInfo))
            {
                if (throwIfInvalid)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ExceptionInvalidProperty,
                            propertyName,
                            type.FullName));
                }
                else
                {
                    return null;
                }
            }

            return propertyInfo;
        }

        internal static bool IsValidProperty(PropertyInfo propertyInfo)
        {
            return null != propertyInfo				// exists
                    && propertyInfo.CanRead			// and it's readable
                    && propertyInfo.GetIndexParameters().Length == 0;	// and it's not an indexer
        }

        internal static FieldInfo GetField(Type type, string fieldName, bool throwIfInvalid)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }

            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

            if (!IsValidField(fieldInfo))
            {
                if (throwIfInvalid)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ExceptionInvalidField,
                            fieldName,
                            type.FullName));
                }
                else
                {
                    return null;
                }
            }

            return fieldInfo;
        }

        internal static bool IsValidField(FieldInfo fieldInfo)
        {
            return null != fieldInfo;
        }

        internal static MethodInfo GetMethod(Type type, string methodName, bool throwIfInvalid)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentNullException("methodName");
            }

            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

            if (!IsValidMethod(methodInfo))
            {
                if (throwIfInvalid)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ExceptionInvalidMethod,
                            methodName,
                            type.FullName));
                }
                else
                {
                    return null;
                }
            }

            return methodInfo;
        }

        internal static bool IsValidMethod(MethodInfo methodInfo)
        {
            return null != methodInfo
                && typeof(void) != methodInfo.ReturnType
                && methodInfo.GetParameters().Length == 0;
        }

        internal static T ExtractValidationAttribute<T>(MemberInfo attributeProvider, string ruleset)
            where T : BaseValidationAttribute
        {
            if (attributeProvider != null)
            {
                foreach (T attribute in GetCustomAttributes(attributeProvider, typeof(T), false))
                {
                    if (ruleset.Equals(attribute.Ruleset))
                    {
                        return attribute;
                    }
                }
            }

            return null;
        }

        internal static T ExtractValidationAttribute<T>(ParameterInfo attributeProvider, string ruleset)
            where T : BaseValidationAttribute
        {
            if (attributeProvider != null)
            {
                foreach (T attribute in attributeProvider.GetCustomAttributes(typeof(T), false))
                {
                    if (ruleset.Equals(attribute.Ruleset))
                    {
                        return attribute;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves an array of the custom attributes applied to a member of a type, looking for the existence
        /// of a metadata type where the attributes are actually specified.
        /// Parameters specify the member, the type of the custom attribute to search
        /// for, and whether to search ancestors of the member.
        /// </summary>
        /// <param name="element">An object derived from the <see cref="MemberInfo"/> class that describes a 
        /// constructor, event, field, method, or property member of a class.</param>
        /// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
        /// <param name="inherit">If <see langword="true"/>, specifies to also search the ancestors of element for 
        /// custom attributes.</param>
        /// <returns>An <see cref="Attribute"/> array that contains the custom attributes of type type applied to 
        /// element, or an empty array if no such custom attributes exist.</returns>
        /// <seealso cref="MetadataTypeAttribute"/>
        public static Attribute[] GetCustomAttributes(MemberInfo element, Type attributeType, bool inherit)
        {
            MemberInfo matchingElement = GetMatchingElement(element);

            return Attribute.GetCustomAttributes(matchingElement, attributeType, inherit);
        }

        private static MemberInfo GetMatchingElement(MemberInfo element)
        {
            Type sourceType = element as Type;
            bool elementIsType = sourceType != null;
            if (sourceType == null)
            {
                sourceType = element.DeclaringType;
            }

            MetadataTypeAttribute metadataTypeAttribute = (MetadataTypeAttribute)
                Attribute.GetCustomAttribute(sourceType, typeof(MetadataTypeAttribute), false);

            if (metadataTypeAttribute == null)
            {
                return element;
            }

            sourceType = metadataTypeAttribute.MetadataClassType;

            if (elementIsType)
            {
                return sourceType;
            }

            MemberInfo[] matchingMembers =
                sourceType.GetMember(
                    element.Name,
                    element.MemberType,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (matchingMembers.Length > 0)
            {
                MethodBase methodBase = element as MethodBase;
                if (methodBase == null)
                {
                    return matchingMembers[0];
                }

                Type[] parameterTypes = methodBase.GetParameters().Select(pi => pi.ParameterType).ToArray();
                return matchingMembers.Cast<MethodBase>().FirstOrDefault(mb => MatchMethodBase(mb, parameterTypes)) ?? element;
            }

            return element;
        }

        private static bool MatchMethodBase(MethodBase mb, Type[] parameterTypes)
        {
            ParameterInfo[] parameters = mb.GetParameters();

            if (parameters.Length != parameterTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != parameterTypes[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
