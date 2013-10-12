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
using System.Collections.Generic;
using System.Reflection;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// Encapsulates shared validator building behavior.
    /// </summary>
    /// <remarks>
    /// This class relies on implementations of <see cref="IValidatedType"/> to supply validation descriptors.
    /// </remarks>
    public class ValidatorBuilderBase
    {
        private static readonly MemberAccessValidatorBuilderFactory DefaultMemberAccessValidatorFactory =
            new MemberAccessValidatorBuilderFactory();

        private readonly MemberAccessValidatorBuilderFactory memberAccessValidatorFactory;
        private readonly ValidatorFactory validatorFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberAccessValidatorFactory"></param>
        /// <param name="validatorFactory"></param>
        public ValidatorBuilderBase(
            MemberAccessValidatorBuilderFactory memberAccessValidatorFactory,
            ValidatorFactory validatorFactory)
        {
            this.memberAccessValidatorFactory = memberAccessValidatorFactory;
            this.validatorFactory = validatorFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validatedType"></param>
        /// <returns></returns>
        public Validator CreateValidator(IValidatedType validatedType)
        {
            List<Validator> validators = new List<Validator>();

            CollectValidatorsForType(validatedType, validators);
            CollectValidatorsForProperties(validatedType.GetValidatedProperties(), validators, validatedType.TargetType);
            CollectValidatorsForFields(validatedType.GetValidatedFields(), validators, validatedType.TargetType);
            CollectValidatorsForMethods(validatedType.GetValidatedMethods(), validators, validatedType.TargetType);
            CollectValidatorsForSelfValidationMethods(validatedType.GetSelfValidationMethods(), validators);

            if (validators.Count == 1)
            {
                return validators[0];
            }
            else
            {
                return new AndCompositeValidator(validators.ToArray());
            }
        }

        private void CollectValidatorsForType(IValidatedType validatedType, List<Validator> validators)
        {
            Validator validator = CreateValidatorForValidatedElement(validatedType, this.GetCompositeValidatorBuilderForType);

            if (validator != null)
            {
                validators.Add(validator);
            }
        }

        private void CollectValidatorsForProperties(IEnumerable<IValidatedElement> validatedElements,
            List<Validator> validators,
            Type ownerType)
        {
            foreach (IValidatedElement validatedElement in validatedElements)
            {
                Validator validator = CreateValidatorForValidatedElement(validatedElement,
                    this.GetCompositeValidatorBuilderForProperty);

                if (validator != null)
                {
                    validators.Add(validator);
                }
            }
        }

        private void CollectValidatorsForFields(IEnumerable<IValidatedElement> validatedElements,
            List<Validator> validators,
            Type ownerType)
        {
            foreach (IValidatedElement validatedElement in validatedElements)
            {
                Validator validator = CreateValidatorForValidatedElement(validatedElement,
                    this.GetCompositeValidatorBuilderForField);

                if (validator != null)
                {
                    validators.Add(validator);
                }
            }
        }

        private void CollectValidatorsForMethods(IEnumerable<IValidatedElement> validatedElements,
            List<Validator> validators,
            Type ownerType)
        {
            foreach (IValidatedElement validatedElement in validatedElements)
            {
                Validator validator = CreateValidatorForValidatedElement(validatedElement,
                    this.GetCompositeValidatorBuilderForMethod);

                if (validator != null)
                {
                    validators.Add(validator);
                }
            }
        }

        private void CollectValidatorsForSelfValidationMethods(IEnumerable<MethodInfo> selfValidationMethods, List<Validator> validators)
        {
            foreach (MethodInfo selfValidationMethod in selfValidationMethods)
            {
                validators.Add(new SelfValidationValidator(selfValidationMethod));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validatedElement"></param>
        /// <param name="validatorBuilderCreator"></param>
        /// <returns></returns>
        protected Validator CreateValidatorForValidatedElement(
            IValidatedElement validatedElement,
            CompositeValidatorBuilderCreator validatorBuilderCreator)
        {
            IEnumerator<IValidatorDescriptor> validatorDescriptorsEnumerator =
                validatedElement.GetValidatorDescriptors().GetEnumerator();

            if (!validatorDescriptorsEnumerator.MoveNext())
            {
                return null;
            }

            CompositeValidatorBuilder validatorBuilder = validatorBuilderCreator(validatedElement);

            do
            {
                Validator validator =
                    validatorDescriptorsEnumerator.Current
                        .CreateValidator(
                            validatedElement.TargetType,
                            validatedElement.MemberInfo.ReflectedType,
                            this.memberAccessValidatorFactory.MemberValueAccessBuilder,
                            this.validatorFactory);
                validatorBuilder.AddValueValidator(validator);
            }
            while (validatorDescriptorsEnumerator.MoveNext());

            return validatorBuilder.GetValidator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validatedElement"></param>
        /// <returns></returns>
        protected delegate CompositeValidatorBuilder CompositeValidatorBuilderCreator(IValidatedElement validatedElement);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validatedElement"></param>
        /// <returns></returns>
        protected CompositeValidatorBuilder GetCompositeValidatorBuilderForProperty(IValidatedElement validatedElement)
        {
            if (validatedElement == null) throw new ArgumentNullException("validatedElement");

            return this.memberAccessValidatorFactory.GetPropertyValueAccessValidatorBuilder(validatedElement.MemberInfo as PropertyInfo,
                validatedElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validatedElement"></param>
        /// <returns></returns>
        protected CompositeValidatorBuilder GetValueCompositeValidatorBuilderForProperty(IValidatedElement validatedElement)
        {
            return new CompositeValidatorBuilder(validatedElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validatedElement"></param>
        /// <returns></returns>
        protected CompositeValidatorBuilder GetCompositeValidatorBuilderForField(IValidatedElement validatedElement)
        {
            if (validatedElement == null) throw new ArgumentNullException("validatedElement");

            return this.memberAccessValidatorFactory.GetFieldValueAccessValidatorBuilder(validatedElement.MemberInfo as FieldInfo,
                validatedElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validatedElement"></param>
        /// <returns></returns>
        protected CompositeValidatorBuilder GetCompositeValidatorBuilderForMethod(IValidatedElement validatedElement)
        {
            if (validatedElement == null) throw new ArgumentNullException("validatedElement");

            return this.memberAccessValidatorFactory.GetMethodValueAccessValidatorBuilder(validatedElement.MemberInfo as MethodInfo,
                validatedElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validatedElement"></param>
        /// <returns></returns>
        protected CompositeValidatorBuilder GetCompositeValidatorBuilderForType(IValidatedElement validatedElement)
        {
            if (validatedElement == null) throw new ArgumentNullException("validatedElement");

            return this.memberAccessValidatorFactory.GetTypeValidatorBuilder(validatedElement.MemberInfo as Type,
                validatedElement);
        }

        /// <summary>
        /// 
        /// </summary>
        protected ValidatorFactory ValidatorFactory
        {
            get { return this.validatorFactory; }
        }
    }
}
