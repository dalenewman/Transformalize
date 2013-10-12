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
using System.Diagnostics;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Defines information needed to install a <see cref="PerformanceCounterCategory"></see>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class PerformanceCountersDefinitionAttribute : Attribute
    {
        PerformanceCounterCategoryType categoryType;
        string categoryName;
        string categoryHelp;

        /// <summary>
        /// Gets the <see cref="PerformanceCounter"></see> category type.
        /// </summary>
        public PerformanceCounterCategoryType CategoryType
        {
            get { return categoryType; }
        }

        /// <summary>
        /// Gets the <see cref="PerformanceCounter"></see> category name.
        /// </summary>
        public string CategoryName
        {
            get { return categoryName; }
        }

        /// <summary>
        /// Gets the <see cref="PerformanceCounter"></see> category help resource name.
        /// This is not the help text itself, 
        /// but is the resource name used to look up the internationalized help text at install-time.
        /// </summary>
        public string CategoryHelp
        {
            get { return categoryHelp; }
        }

        /// <overloads>
        /// Initializes this attribute with information needed to install this performance counter category.
        /// </overloads>
        /// <summary>
        /// Initializes this attribute with information needed to install this performance counter category.
        /// </summary>
        /// <param name="categoryName">Performance counter category name</param>
        /// <param name="categoryHelp">Counter category help resource name. 
        /// This is not the help text itself, 
        /// but is the resource name used to look up the internationalized help text at install-time.
        ///</param>
        public PerformanceCountersDefinitionAttribute(string categoryName, string categoryHelp)
            : this(categoryName, categoryHelp, PerformanceCounterCategoryType.MultiInstance)
        {
        }

        /// <summary>
        /// Initializes this attribute with information needed to install this performance counter category.
        /// </summary>
        /// <param name="categoryName">Performance counter category name</param>
        /// <param name="categoryHelp">Counter category help resource name. 
        /// This is not the help text itself, 
        /// but is the resource name used to look up the internationalized help text at install-time.
        ///</param>
        /// <param name="categoryType">Performance counter category type.</param>
        public PerformanceCountersDefinitionAttribute(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType)
        {
            this.categoryName = categoryName;
            this.categoryHelp = categoryHelp;
            this.categoryType = categoryType;
        }
    }
}
