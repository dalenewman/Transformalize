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
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.Security;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Instrumentation
{
    /// <summary>
    /// Generic installer wrapper around installer builder. Used to find and install 
    /// given type of installable resource.
    /// </summary>
    /// <typeparam name="TInstallerBuilder">Specific type of installer builder to instantiate</typeparam>
    [SecurityCritical]
    public class ReflectionInstaller<TInstallerBuilder> : Installer
        where TInstallerBuilder : AbstractInstallerBuilder
    {
        /// <summary>
        /// Installs the instrumentation resources
        /// </summary>
        /// <param name="stateSaver">An <see cref="IDictionary"/> used to save information needed to perform a commit, rollback, or uninstall operation.</param>
        [SecurityCritical]
        public override void Install(IDictionary stateSaver)
        {
            PrepareInstaller();
            base.Install(stateSaver);
        }

        /// <summary>
        /// Uninstalls the instrumentation resources
        /// </summary>
        /// <param name="stateSaver">An <see cref="IDictionary"/> that contains the state of the computer after the installation was complete.</param>
        [SecurityCritical]
        public override void Uninstall(IDictionary stateSaver)
        {
            PrepareInstaller();
            base.Uninstall(stateSaver);
        }

        private void PrepareInstaller()
        {
            string assemblyName = this.Context.Parameters["assemblypath"];
            Type[] types = Assembly.LoadFile(assemblyName).GetTypes();

            TInstallerBuilder builder = (TInstallerBuilder)Activator.CreateInstance(typeof(TInstallerBuilder), new object[] { types });

            builder.Fill(this);
        }
    }
}
