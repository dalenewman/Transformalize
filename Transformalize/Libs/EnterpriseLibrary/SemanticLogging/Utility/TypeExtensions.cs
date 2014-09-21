#region license
// ==============================================================================
// Microsoft patterns & practices Enterprise Library
// Semantic Logging Application Block
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
#endregion

using System;
using System.ComponentModel;

namespace Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Utility
{
    internal static class TypeExtensions
    {
        public static object Default(this Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }

            return Activator.CreateInstance(type);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.ComponentModel.TypeConverter.ConvertFromInvariantString(System.String)", Justification = "Converting numeri value")]
        public static object NotDefault(this Type type)
        {
            if (type == typeof(string))
            {
                return "1";
            }

            if (type == typeof(Guid))
            {
                return Guid.NewGuid();
            }

            if (type == typeof(bool))
            {
                return true;
            }

            TypeConverter tc = TypeDescriptor.GetConverter(type);
            if (tc != null && tc.CanConvertFrom(typeof(string)))
            {
                return tc.ConvertFromInvariantString("1");
            }

            return Activator.CreateInstance(type);
        }

        public static bool IsDefault(this object value)
        {
            if (value == null)
            {
                return true;
            }

            return value.Equals(value.GetType().Default());
        }
    }
}
