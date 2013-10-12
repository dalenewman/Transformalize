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

using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public class ValueAccessValidatorBuilder : CompositeValidatorBuilder
    {
        private ValueAccess valueAccess;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueAccess"></param>
        /// <param name="validatedElement"></param>
        public ValueAccessValidatorBuilder(ValueAccess valueAccess, IValidatedElement validatedElement)
            : base(validatedElement)
        {
            this.valueAccess = valueAccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Validator DoGetValidator()
        {
            return new ValueAccessValidator(this.valueAccess, base.DoGetValidator());
        }

        #region test only properties

        /// <summary>
        /// 
        /// </summary>
        public ValueAccess ValueAccess
        {
            get { return this.valueAccess; }
        }

        #endregion
    }
}
