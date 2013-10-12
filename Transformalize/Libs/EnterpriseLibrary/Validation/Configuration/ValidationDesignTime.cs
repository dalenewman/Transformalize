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

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
    internal static class ValidationDesignTime
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class ViewModelTypeNames
        {
            public const string ValidationSectionViewModel = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ValidationSectionViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            public const string ValidatedTypeReferenceViewModel = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ValidatedTypeReferenceViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            public const string ValidatorDataViewModel = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ValidatorDataViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
            
            public const string ValidationRulesetDataViewModel = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ValidationRulesetDataViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            public const string ValidatedMemberReferenceViewModel = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ValidatedMemberReferenceViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            public const string DomainConfigurationElementViewModel =
                "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.DomainConfigurationElementViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            public const string RangeValidatorCultureProperty = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.RangeValidatorCultureProperty, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class CommandTypeNames
        {
            public const string SelectValidatedTypeReferenceMembersCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.SelectValidatedTypeReferenceMembersCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            public const string AddValidatedTypeCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ValidationTypeReferenceAddCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Validators
        {
            public const string NameValueCollectionValidator = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.Validation.NameValueCollectionValidator, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            public const string RangeBoundValidator = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.RangeBoundValidator, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
        }
    }
}
