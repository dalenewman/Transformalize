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
using System.Configuration;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    ///<summary>
    /// Indicates that this assembly handles the <see cref="ConfigurationSection"/>.
    ///</summary>
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple=true)]
    public class HandlesSectionAttribute : Attribute
    {
        private readonly string sectionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlesSectionAttribute"/> class.
        /// </summary>
        /// <param name="sectionName"></param>
        public HandlesSectionAttribute(string sectionName)
        {
            this.sectionName = sectionName;
        }

        ///<summary>
        /// Name of the section handled by this assembly.
        ///</summary>
        public string SectionName
        {
            get { return sectionName; }
        }

        /// <summary>
        /// Indicates this section should be cleared during save, but there is no 
        /// direct handler for it.
        /// </summary>
        public bool ClearOnly { get; set; }
    }
}
