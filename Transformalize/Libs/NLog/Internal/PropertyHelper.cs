#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Conditions;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Reflection helpers for accessing properties.
    /// </summary>
    internal static class PropertyHelper
    {
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> parameterInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        internal static void SetPropertyFromString(object o, string name, string value, ConfigurationItemFactory configurationItemFactory)
        {
            InternalLogger.Debug("Setting '{0}.{1}' to '{2}'", o.GetType().Name, name, value);

            PropertyInfo propInfo;

            if (!TryGetPropertyInfo(o, name, out propInfo))
            {
                throw new NotSupportedException("Parameter " + name + " not supported on " + o.GetType().Name);
            }

            try
            {
                if (propInfo.IsDefined(typeof (ArrayParameterAttribute), false))
                {
                    throw new NotSupportedException("Parameter " + name + " of " + o.GetType().Name + " is an array and cannot be assigned a scalar value.");
                }

                object newValue;

                var propertyType = propInfo.PropertyType;

                propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                if (!TryNLogSpecificConversion(propertyType, value, out newValue, configurationItemFactory))
                {
                    if (!TryGetEnumValue(propertyType, value, out newValue))
                    {
                        if (!TryImplicitConversion(propertyType, value, out newValue))
                        {
                            if (!TrySpecialConversion(propertyType, value, out newValue))
                            {
                                newValue = Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }

                propInfo.SetValue(o, newValue, null);
            }
            catch (TargetInvocationException ex)
            {
                throw new NLogConfigurationException("Error when setting property '" + propInfo.Name + "' on " + o, ex.InnerException);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                throw new NLogConfigurationException("Error when setting property '" + propInfo.Name + "' on " + o, exception);
            }
        }

        internal static bool IsArrayProperty(Type t, string name)
        {
            PropertyInfo propInfo;

            if (!TryGetPropertyInfo(t, name, out propInfo))
            {
                throw new NotSupportedException("Parameter " + name + " not supported on " + t.Name);
            }

            return propInfo.IsDefined(typeof (ArrayParameterAttribute), false);
        }

        internal static bool TryGetPropertyInfo(object o, string propertyName, out PropertyInfo result)
        {
            var propInfo = o.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propInfo != null)
            {
                result = propInfo;
                return true;
            }

            lock (parameterInfoCache)
            {
                var targetType = o.GetType();
                Dictionary<string, PropertyInfo> cache;

                if (!parameterInfoCache.TryGetValue(targetType, out cache))
                {
                    cache = BuildPropertyInfoDictionary(targetType);
                    parameterInfoCache[targetType] = cache;
                }

                return cache.TryGetValue(propertyName, out result);
            }
        }

        internal static Type GetArrayItemType(PropertyInfo propInfo)
        {
            var arrayParameterAttribute = (ArrayParameterAttribute) Attribute.GetCustomAttribute(propInfo, typeof (ArrayParameterAttribute));
            if (arrayParameterAttribute != null)
            {
                return arrayParameterAttribute.ItemType;
            }

            return null;
        }

        internal static IEnumerable<PropertyInfo> GetAllReadableProperties(Type type)
        {
#if NETCF2_0
    // .NET Compact Framework 2.0 understands 'Public' differently
    // it only returns properties where getter and setter are public

            var allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            var readableProperties = new List<PropertyInfo>();
            foreach (var prop in allProperties)
            {
                if (prop.CanRead)
                {
                    readableProperties.Add(prop);
                }
            }

            return readableProperties;
#else
            // other frameworks don't have this problem
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
#endif
        }

        internal static void CheckRequiredParameters(object o)
        {
            foreach (var propInfo in GetAllReadableProperties(o.GetType()))
            {
                if (propInfo.IsDefined(typeof (RequiredParameterAttribute), false))
                {
                    var value = propInfo.GetValue(o, null);
                    if (value == null)
                    {
                        throw new NLogConfigurationException(
                            "Required parameter '" + propInfo.Name + "' on '" + o + "' was not specified.");
                    }
                }
            }
        }

        private static bool TryImplicitConversion(Type resultType, string value, out object result)
        {
            var operatorImplicitMethod = resultType.GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, null, new[] {typeof (string)}, null);
            if (operatorImplicitMethod == null)
            {
                result = null;
                return false;
            }

            result = operatorImplicitMethod.Invoke(null, new object[] {value});
            return true;
        }

        private static bool TryNLogSpecificConversion(Type propertyType, string value, out object newValue, ConfigurationItemFactory configurationItemFactory)
        {
            if (propertyType == typeof (Layout) || propertyType == typeof (SimpleLayout))
            {
                newValue = new SimpleLayout(value, configurationItemFactory);
                return true;
            }

            if (propertyType == typeof (ConditionExpression))
            {
                newValue = ConditionParser.ParseExpression(value, configurationItemFactory);
                return true;
            }

            newValue = null;
            return false;
        }

        private static bool TryGetEnumValue(Type resultType, string value, out object result)
        {
            if (!resultType.IsEnum)
            {
                result = null;
                return false;
            }

            if (resultType.IsDefined(typeof (FlagsAttribute), false))
            {
                ulong union = 0;

                foreach (var v in value.Split(','))
                {
                    var enumField = resultType.GetField(v.Trim(), BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                    if (enumField == null)
                    {
                        throw new NLogConfigurationException("Invalid enumeration value '" + value + "'.");
                    }

                    union |= Convert.ToUInt64(enumField.GetValue(null), CultureInfo.InvariantCulture);
                }

                result = Convert.ChangeType(union, Enum.GetUnderlyingType(resultType), CultureInfo.InvariantCulture);
                result = Enum.ToObject(resultType, result);

                return true;
            }
            else
            {
                var enumField = resultType.GetField(value, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                if (enumField == null)
                {
                    throw new NLogConfigurationException("Invalid enumeration value '" + value + "'.");
                }

                result = enumField.GetValue(null);
                return true;
            }
        }

        private static bool TrySpecialConversion(Type type, string value, out object newValue)
        {
            if (type == typeof (Uri))
            {
                newValue = new Uri(value, UriKind.RelativeOrAbsolute);
                return true;
            }

            if (type == typeof (Encoding))
            {
                newValue = Encoding.GetEncoding(value);
                return true;
            }

            if (type == typeof (CultureInfo))
            {
                newValue = new CultureInfo(value);
                return true;
            }

            if (type == typeof (Type))
            {
                newValue = Type.GetType(value, true);
                return true;
            }

            newValue = null;
            return false;
        }

        private static bool TryGetPropertyInfo(Type targetType, string propertyName, out PropertyInfo result)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                var propInfo = targetType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    result = propInfo;
                    return true;
                }
            }

            lock (parameterInfoCache)
            {
                Dictionary<string, PropertyInfo> cache;

                if (!parameterInfoCache.TryGetValue(targetType, out cache))
                {
                    cache = BuildPropertyInfoDictionary(targetType);
                    parameterInfoCache[targetType] = cache;
                }

                return cache.TryGetValue(propertyName, out result);
            }
        }

        private static Dictionary<string, PropertyInfo> BuildPropertyInfoDictionary(Type t)
        {
            var retVal = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var propInfo in GetAllReadableProperties(t))
            {
                var arrayParameterAttribute = (ArrayParameterAttribute) Attribute.GetCustomAttribute(propInfo, typeof (ArrayParameterAttribute));

                if (arrayParameterAttribute != null)
                {
                    retVal[arrayParameterAttribute.ElementName] = propInfo;
                }
                else
                {
                    retVal[propInfo.Name] = propInfo;
                }

                if (propInfo.IsDefined(typeof (DefaultParameterAttribute), false))
                {
                    // define a property with empty name
                    retVal[string.Empty] = propInfo;
                }
            }

            return retVal;
        }
    }
}