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
using System.ComponentModel;
using System.Globalization;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation intended to convert to and from <see cref="CultureInfo"/> instances when serializing and de-serializing from configuration.
    /// </summary>
    public class ConfigurationCultureInfoConverter : TypeConverter
    {
        /// <summary/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        /// <summary/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string && !string.IsNullOrEmpty((string)value))
            {
                return CultureInfo.GetCultureInfo((string)value);
            }
            return null;
        }

        /// <summary/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string));
        }

        /// <summary/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == CultureInfo.InvariantCulture) throw new ArgumentException(Resources.InvariantCultureCannotBeUsedToDeserializeConfiguration, "culture");

            if (value is CultureInfo && destinationType == typeof(string))
            {
                return ((CultureInfo)value).Name;
            }
            return null;
        }
    }
}
