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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Performs validation by invoking a method on the validated object.
    /// </summary>
    public class SelfValidationValidator : Validator
    {
        private MethodInfo methodInfo;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="SelfValidationValidator"/> class with the 
        /// method that is to be invoked when performing validation.</para>
        /// </summary>
        /// <param name="methodInfo">The self validation method to invoke.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="methodInfo"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="methodInfo"/> does not have the required signature.</exception>
        public SelfValidationValidator(MethodInfo methodInfo)
            : base(null, null)
        {
            if (null == methodInfo)
            {
                throw new ArgumentNullException("methodInfo");
            }

            if (typeof(void) != methodInfo.ReturnType)
            {
                throw new ArgumentException(Resources.ExceptionSelfValidationMethodWithInvalidSignature, "methodInfo");
            }

            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (1 != parameters.Length || typeof(ValidationResults) != parameters[0].ParameterType)
                throw new ArgumentException(Resources.ExceptionSelfValidationMethodWithInvalidSignature, "methodInfo");

            this.methodInfo = methodInfo;
        }

        /// <summary>
        /// Validates by invoking the self validation method configured for the validator.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// A validation failure will be logged by the validator when the following conditions are met without
        /// invoking the self validation method:
        /// <list type="bullet">
        /// <item><term><paramref name="objectToValidate"/> is <see langword="null"/>.</term></item>
        /// <item><term><paramref name="objectToValidate"/> is an instance of a type not compatible with the declaring type for the 
        /// self validation method.</term></item>
        /// </list>
        /// <para/>
        /// A validation failure will also be logged if the validation method throws an exception.
        /// </remarks>
        public override void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            if (null == objectToValidate)
            {
                this.LogValidationResult(validationResults, Resources.SelfValidationValidatorMessage, currentTarget, key);
            }
            else if (!this.methodInfo.DeclaringType.IsAssignableFrom(objectToValidate.GetType()))
            {
                this.LogValidationResult(validationResults, Resources.SelfValidationValidatorMessage, currentTarget, key);
            }
            else
            {
                try
                {
                    this.methodInfo.Invoke(objectToValidate, new object[] { validationResults });
                }
                catch (Exception)
                {
                    this.LogValidationResult(validationResults, Resources.SelfValidationMethodThrownMessage, currentTarget, key);
                }
            }
        }

        /// <summary>
        /// Gets the message template to use when logging results no message is supplied.
        /// </summary>
        protected override string DefaultMessageTemplate
        {
            get { return null; }
        }
    }
}
