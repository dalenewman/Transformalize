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
using System.Resources;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    /// <summary>
    /// Attribute used to decorate a designtime View Model element with an executable command. E.g. a context menu item that allows
    /// the user to perform an action in the elements context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        private Guid typeId;
        private string title;
        private bool resourceLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class, specifying the Command Model Type.
        /// </summary>
        /// <remarks>
        /// The Command Model Type should derive from the CommandModel class in the Configuration.Design assembly. <br/>
        /// As this attribute can be applied to the configuration directly and we dont want to force a dependency on the Configuration.Design assembly <br/>
        /// You can specify the Command Model Type in a loosy coupled fashion.
        /// </remarks>
        /// <param name="commandModelTypeName">The fully qualified name of the Command Model Type.</param>
        public CommandAttribute(string commandModelTypeName)
        {
            if (string.IsNullOrEmpty(commandModelTypeName)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "commandModelTypeName");

            this.CommandModelTypeName = commandModelTypeName;
            this.Replace = CommandReplacement.NoCommand;
            this.CommandPlacement = CommandPlacement.ContextCustom;

            this.typeId = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class, specifying the Command Model Type.
        /// </summary>
        /// <remarks>
        /// The Command Model Type should derive from the CommandModel class in the Configuration.Design assmbly. <br/>
        /// As this attribute can be applied to the configuration directly and we dont want to force a dependency on the Configuration.Design assembly <br/>
        /// You can specify the Command Model Type in a loosy coupled fashion.
        /// </remarks>
        /// <param name="commandModelType">The Command Model Type.</param>
        public CommandAttribute(Type commandModelType)
            : this(commandModelType != null ? commandModelType.AssemblyQualifiedName : string.Empty)
        {
        }

        /// <summary>
        /// Gets or sets the name of the resource, used to return a localized title that will be shown for this command in the UI (User Interface).
        /// </summary>
        public string TitleResourceName { get; set; }

        /// <summary>
        /// Gets or sets the type of the resource, used to return a localized title that will be shown for this command in the UI (User Interface).
        /// </summary>
        public Type TitleResourceType { get; set; }

        /// <summary>
        /// Gets the title that will be shown for this command in the UI (User Interface).
        /// </summary>
        public virtual string Title
        {
            get
            {
                if (TitleResourceName != null && TitleResourceType != null)
                {
                    EnsureTitleLoaded();
                }
                return title;
            }
            set
            {
                title = value;
            }
        }

        private void EnsureTitleLoaded()
        {
            if (resourceLoaded) return;

            var rm = new ResourceManager(TitleResourceType);

            try
            {
                title = rm.GetString(TitleResourceName);
            }
            catch (MissingManifestResourceException)
            {
                title = TitleResourceName;
            }

            resourceLoaded = true;
        }

        /// <summary>
        /// Gets or sets the <see cref="CommandReplacement"/> options for this command.
        /// </summary>
        public CommandReplacement Replace { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CommandPlacement"/> options for this command.
        /// </summary>
        public CommandPlacement CommandPlacement { get; set; }

        /// <summary>
        /// Gets or Sets the Command Model Type Name for this command. <br/>
        /// The Command Model Type will be used at runtime to display and execute the command.<br/>
        /// Command Model Types should derive from the CommandModel class in the Configuration.Design assembly. 
        /// </summary>
        public string CommandModelTypeName { get; set; }

        /// <summary>
        /// Gets the Command Model Type for this command. <br/>
        /// The Command Model Type will be used at runtime to display and execute the command.<br/>
        /// Command Model Types should derive from the CommandModel class in the Configuration.Design assembly. 
        /// </summary>
        public Type CommandModelType
        {
            get { return Type.GetType(CommandModelTypeName, true); }
        }

        /// <summary>
        /// Defines the keyboard gesture for this command.
        /// </summary>
        /// <example>
        ///     command.KeyGesture = "Ctrl+1";
        /// </example>
        public string KeyGesture { get; set; }

        /// <summary>
        /// When implemented in a derived class, gets a unique identifier for this <see cref="T:System.Attribute"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that is a unique identifier for the attribute.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object TypeId
        {
            get
            {
                //the identity of a CommandAttribute is either the command it replaces or unique value per instance.
                if (Replace == CommandReplacement.NoCommand)
                {
                    return typeId;
                }
                return Replace;
            }
        }

    }

    /// <summary>
    /// Specifies whether a command replaces a default command.
    /// </summary>
    public enum CommandReplacement
    {
        /// <summary>
        /// Specifies that the command should be used to replace the default add command.
        /// </summary>
        DefaultAddCommandReplacement,

        /// <summary>
        /// Specifies that the command should be used to replace the default delete command.
        /// </summary>
        DefaultDeleteCommandReplacement,

        /// <summary>
        /// Specifies that the command should not be used to replace any default command.
        /// </summary>
        NoCommand
    }

    /// <summary>
    /// Specifies the placement of a command. This can be either a top level menu, e.g.: <see cref="CommandPlacement.FileMenu"/> or <see cref="CommandPlacement.BlocksMenu"/> or
    /// a context menu, e.g.: <see cref="CommandPlacement.ContextAdd"/>,  <see cref="CommandPlacement.ContextCustom"/>.
    /// </summary>
    public enum CommandPlacement
    {
        /// <summary>
        /// Specifies placement of the command in the top level file menu.
        /// </summary>
        FileMenu,

        /// <summary>
        /// Specifies placement of the command in the top level blocks menu.
        /// </summary>
        BlocksMenu,

        /// <summary>
        /// Specifies placement of the command in the top level wizards menu.
        /// </summary>
        WizardMenu,

        /// <summary>
        /// Specifies placement of the command in the contextual add menu for an configuration element.
        /// </summary>
        ContextAdd,

        /// <summary>
        /// Specifies placement of the command in the custom commands menu for an configuration element.
        /// </summary>
        ContextCustom,

        /// <summary>
        /// Specifies placement of the command in the delete commands menu for an configuration element.
        /// </summary>
        ContextDelete,

    }
}
