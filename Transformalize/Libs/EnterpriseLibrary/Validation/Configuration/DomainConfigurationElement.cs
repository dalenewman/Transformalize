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

using Transformalize.Libs.EnterpriseLibrary.Common.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Libs.EnterpriseLibrary.Validation.Configuration
{
	/// <summary>
	/// Represents an individual domain element for a <see cref="DomainValidator{T}"/>.
	/// </summary>
    [ResourceDescription(typeof(DesignResources), "DomainConfigurationElementDescription")]
    [ResourceDisplayName(typeof(DesignResources), "DomainConfigurationElementDisplayName")]
    public class DomainConfigurationElement : NamedConfigurationElement
	{
		/// <summary>
		/// Initialize a new instance of a <see cref="DomainConfigurationElement"/> class.
		/// </summary>
		public DomainConfigurationElement()
		{ }

		/// <summary>
		/// Intialize a new instance of a <see cref="DomainConfigurationElement"/> class with a name.
		/// </summary>
		/// <param name="name">The name of the element.</param>
		public DomainConfigurationElement(string name)
			: base(name)
		{ }

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        /// <remarks>
        /// Overriden in order to annotate with designtime attribute.
        /// </remarks>
        [ViewModel(CommonDesignTime.ViewModelTypeNames.CollectionEditorContainedElementProperty)]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
	}
}
