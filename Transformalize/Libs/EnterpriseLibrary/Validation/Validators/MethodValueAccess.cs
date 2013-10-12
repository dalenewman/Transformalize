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
    /// Represents the logic to access values from a method.
    /// </summary>
    /// <seealso cref="ValueAccess"/>
    public sealed class MethodValueAccess : ValueAccess
    {
        private MethodInfo methodInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        public MethodValueAccess(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
        }

        /// <summary>
        /// Retrieves a value from <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source for the value.</param>
        /// <param name="value">The value retrieved from the <paramref name="source"/>.</param>
        /// <param name="valueAccessFailureMessage">A message describing the failure to access the value, if any.</param>
        /// <returns><see langword="true"/> when the retrieval was successful; <see langword="false"/> otherwise.</returns>
        /// <remarks>Subclasses provide concrete value accessing behaviors.</remarks>
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
            if (!this.methodInfo.DeclaringType.IsAssignableFrom(source.GetType()))
            {
                valueAccessFailureMessage
                    = string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ErrorValueAccessInvalidType,
                        this.Key,
                        source.GetType().FullName);
                return false;
            }

            value = this.methodInfo.Invoke(source, null);
            return true;
        }

        /// <summary>
        /// Key used to retrieve data. In this case, it's just the method name.
        /// </summary>
        public override string Key
        {
            get { return this.methodInfo.Name; }
        }

        #region test only properties

        /// <summary>
        /// 
        /// </summary>
        public MethodInfo MethodInfo
        {
            get { return this.methodInfo; }
        }

        #endregion
    }
}
