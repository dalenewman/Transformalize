//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design.Validation
{
    ///<summary>
    /// Defines the type of attribute to apply this configuration property or field.
    ///</summary>
    /// <remarks>
    /// This attribute is applied to create validators for use in the configuration design-time.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ValidationAttribute : Attribute
    {
        private string validatorType;

        ///<summary>
        /// Creates an instance of ValidationAttribute with the validator type specified by <see cref="string"/>.
        ///</summary>
        public ValidationAttribute(string validatorType)
        {
            if (string.IsNullOrEmpty(validatorType)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "validatorType");

            this.validatorType = validatorType;
        }


        ///<summary>
        /// Creates an instance of the ValidationAttribute with the validator type specified by <see cref="Type"/>
        ///</summary>
        public ValidationAttribute(Type validatorType)
            : this(validatorType != null ? validatorType.AssemblyQualifiedName : null)
        {
        }

        ///<summary>
        /// Retrieves the validator <see cref="Type"/>.
        ///</summary>
        public Type ValidatorType
        {
            get { return Type.GetType(validatorType, true, true); }
        }

        ///<summary>
        /// Creates a validator objects.   This is expected to return a Validator type from
        /// the Microsoft.Practices.EnterpriseLibrary.Configuration.Design namespace.  
        ///</summary>
        ///<returns></returns>
        public object CreateValidator()
        {
            var validatorType = ValidatorType;
            return Activator.CreateInstance(validatorType);
        }

        /// <summary>
        /// When implemented in a derived class, gets a unique identifier for this <see cref="T:System.Attribute"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that is a unique identifier for the attribute.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object TypeId
        {
            get
            {
                return this.validatorType;
            }
        }
    }

    /// <summary>
    /// Indicates an element level validator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class ElementValidationAttribute : Attribute
    {
        private readonly string validatorTypeName;

        ///<summary>
        /// Creates an instance of ElementValidationAttribute with the validator type specified by <see cref="string"/>.
        ///</summary>
        ///<param name="validatorTypeName"></param>
        public ElementValidationAttribute(string validatorTypeName)
        {
            if (String.IsNullOrEmpty(validatorTypeName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "validatorTypeName");

            this.validatorTypeName = validatorTypeName;
        }


        ///<summary>
        /// Creates an instance of the ElementValidationAttribute with the validator type specified by <see cref="Type"/>
        ///</summary>
        ///<param name="validatorType"></param>
        public ElementValidationAttribute(Type validatorType)
            : this(validatorType == null ? null : validatorType.AssemblyQualifiedName)
        {
        }

        ///<summary>
        /// Retrieves the validator <see cref="Type"/>.
        ///</summary>
        public Type ValidatorType
        {
            get { return Type.GetType(validatorTypeName, true, true); }
        }

        ///<summary>
        /// Creates a validator objects.   This is expected to return a Validator type from
        /// the Microsoft.Practices.EnterpriseLibrary.Configuration.Design namespace.  
        ///</summary>
        ///<returns></returns>
        public object CreateValidator()
        {
            var validatorType = ValidatorType;
            return Activator.CreateInstance(validatorType);
        }

        /// <summary>
        /// When implemented in a derived class, gets a unique identifier for this <see cref="T:System.Attribute"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that is a unique identifier for the attribute.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object TypeId
        {
            get
            {
                return this.validatorTypeName;
            }
        }
    }
}
