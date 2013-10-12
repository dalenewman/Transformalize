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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Validators
{
	/// <summary>
	/// Represents a <see cref="ContainsCharactersValidator"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
		Justification = "Fields are used internally")]
	public sealed class ContainsCharactersValidatorAttribute : ValueValidatorAttribute
	{
		private string characterSet;
		private ContainsCharacters containsCharacters;

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ContainsCharactersValidatorAttribute"/> </para>
		/// </summary>
		/// <param name="characterSet">The character set to be evaluated.</param>
		public ContainsCharactersValidatorAttribute(string characterSet)
			: this(characterSet, ContainsCharacters.Any)
		{ }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="ContainsCharactersValidatorAttribute"/> </para>
		/// </summary>
		/// <param name="characterSet">The character set to be evaluated.</param>
		/// <param name="containsCharacters">The mode to evaluate the character set.</param>
		public ContainsCharactersValidatorAttribute(string characterSet, ContainsCharacters containsCharacters)
		{
			ValidatorArgumentsValidatorHelper.ValidateContainsCharacterValidator(characterSet);

			this.characterSet = characterSet;
			this.containsCharacters = containsCharacters;
		}

        /// <summary>
        /// The character set to be evaluated.
        /// </summary>
        public string CharacterSet
        {
            get { return characterSet; }
        }

        /// <summary>
        /// The mode to evaluate the character set.
        /// </summary>
        public ContainsCharacters ContainsCharacters
        {
            get { return containsCharacters; }
        }

        /// <summary>
		/// Creates the <see cref="ContainsCharactersValidator"/> described by the attribute object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <remarks>This operation must be overriden by subclasses.</remarks>
		/// <returns>The created <see cref="ContainsCharactersValidator"/>.</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
            return new ContainsCharactersValidator(CharacterSet, ContainsCharacters, Negated);
		}

        private readonly Guid typeId = Guid.NewGuid();

        /// <summary>
        /// Gets a unique identifier for this attribute.
        /// </summary>
        public override object TypeId
        {
            get
            {
                return this.typeId;
            }
        }
    }
}
