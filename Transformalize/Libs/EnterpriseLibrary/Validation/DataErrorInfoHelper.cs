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
using System.Reflection;
using System.Text;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Helper class to implement the <see cref="System.ComponentModel.IDataErrorInfo"/> interface using the 
    /// Validation Application Block.
    /// </summary>
    public class DataErrorInfoHelper
    {
        private readonly object target;
        private readonly Type targetType;
        private readonly ValidationSpecificationSource source;
        private readonly string ruleset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorInfoHelper"/> class for a target object.
        /// </summary>
        /// <param name="target">The target object.</param>
        public DataErrorInfoHelper(object target)
            : this(target, ValidationSpecificationSource.All, "")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorInfoHelper"/> class for a target object with the 
        /// supplied source and ruleset.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="source">The source for validation rules for the target object.</param>
        /// <param name="ruleset">The ruleset to use when retrieving rules for the target object.</param>
        public DataErrorInfoHelper(object target, ValidationSpecificationSource source, string ruleset)
        {
            if (target == null) throw new ArgumentNullException("target");

            this.target = target;
            this.targetType = target.GetType();
            this.source = source;
            this.ruleset = ruleset;
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <value>An error message indicating what is wrong with this object. The default is an empty string ("").</value>
        public string Error
        {
            get { return ""; }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">The name of the property whose error message to get. </param>
        /// <value>The error message for the property. The default is an empty string ("").</value>
        public string this[string columnName]
        {
            get
            {
                return GetValidationMessage(GetPropertyValidator(columnName));
            }
        }

        private Validator GetPropertyValidator(string propertyName)
        {
            Validator validator = null;

            PropertyInfo property = null;
            try
            {
                property = this.targetType.GetProperty(propertyName);
            }
            catch (ArgumentException) { }
            catch (AmbiguousMatchException) { }
            if (property != null)
            {
                validator =
                    PropertyValidationFactory.GetPropertyValidator(
                        this.targetType,
                        property,
                        this.ruleset,
                        this.source,
                        new ReflectionMemberValueAccessBuilder());
            }

            return validator;
        }

        private string GetValidationMessage(Validator validator)
        {
            if (validator == null)
                return "";

            var results = validator.Validate(this.target);
            if (results.IsValid)
                return "";

            var errorTextBuilder = new StringBuilder();
            foreach (var result in results)
            {
                errorTextBuilder.AppendLine(result.Message);
            }

            return errorTextBuilder.ToString();
        }
    }
}
