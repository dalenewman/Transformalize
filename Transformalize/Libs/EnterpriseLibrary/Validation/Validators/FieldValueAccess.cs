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

using System.Globalization;
using System.Reflection;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Represents the logic to access values from a field.
    /// </summary>
    /// <seealso cref="ValueAccess"/>
    public sealed class FieldValueAccess : ValueAccess
    {
        private FieldInfo fieldInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldInfo"></param>
        public FieldValueAccess(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="valueAccessFailureMessage"></param>
        /// <returns></returns>
        public override bool GetValue(object source, out object value, out string valueAccessFailureMessage)
        {
            value = null;
            valueAccessFailureMessage = null;

            if (null == source)
            {
                valueAccessFailureMessage
                    = string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ErrorValueAccessNull,
                        this.Key);
                return false;
            }
            if (!this.fieldInfo.DeclaringType.IsAssignableFrom(source.GetType()))
            {
                valueAccessFailureMessage
                    = string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ErrorValueAccessInvalidType,
                        this.Key,
                        source.GetType().FullName);
                return false;
            }

            value = this.fieldInfo.GetValue(source);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Key
        {
            get { return this.fieldInfo.Name; }
        }

        #region test only properties

        /// <summary>
        /// 
        /// </summary>
        public FieldInfo FieldInfo
        {
            get { return this.fieldInfo; }
        }

        #endregion
    }
}
