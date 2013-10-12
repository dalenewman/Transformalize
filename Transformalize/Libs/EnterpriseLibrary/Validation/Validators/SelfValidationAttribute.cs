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
	/// Marks a method as implementing self validation logic.
	/// </summary>
	/// <remarks>
	/// The following conditions must be met for a method to be considered for self validation:
	/// <list type="number">
    /// <item><term>The type for which the validation is being created must have the <see cref="HasSelfValidationAttribute"/>.</term></item>
    /// <item><term>The method must have the <see cref="SelfValidationAttribute"/> with configured for the requested ruleset.</term></item>
    /// <item><term>The method must be void and take a single parameter of type <see cref="ValidationResults"/>.</term>term></item>
	/// </list>
	/// <para/>
	/// Non-public methods can be used for self validation, although inherited non-public methods will not be used.
	/// <para/>
	/// There is no configuration based way to specify self validation.
	/// </remarks>
	/// <seealso cref="SelfValidationValidator"/>
	/// <seealso cref="HasSelfValidationAttribute"/>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class SelfValidationAttribute : Attribute
	{
		private string ruleset = string.Empty;

		/// <summary>
		/// Gets or set the ruleset for which the self validation method must be included.
		/// </summary>
		public string Ruleset
		{
			get { return ruleset; }
			set { ruleset = value; }
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
