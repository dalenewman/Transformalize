#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Dynamic;
using Transformalize.Libs.RazorEngine.Compilation;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Provides a base implementation of a template with a model.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public class TemplateBase<T> : TemplateBase, ITemplate<T>
    {
        #region Fields

        private object currentModel;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="TemplateBase{T}" />.
        /// </summary>
        protected TemplateBase()
        {
            HasDynamicModel = GetType().IsDefined(typeof (HasDynamicModelAttribute), true);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Determines whether this template has a dynamic model.
        /// </summary>
        protected bool HasDynamicModel { get; private set; }

        /// <summary>
        ///     Gets or sets the model.
        /// </summary>
        public T Model
        {
            get { return (T) currentModel; }
            set
            {
                if (HasDynamicModel && !(value is DynamicObject) && !(value is ExpandoObject))
                    currentModel = new RazorDynamicObject
                                       {
                                           Model = value
                                       };
                else
                    currentModel = value;
            }
        }

        #endregion

        /// <summary>
        ///     Includes the template with the specified name.
        /// </summary>
        /// <param name="cacheName">The name of the template type in cache.</param>
        /// <param name="model">The model or NULL if there is no model for the template.</param>
        /// <returns>The template writer helper.</returns>
        public override TemplateWriter Include(string cacheName, object model = null)
        {
            return base.Include(cacheName, model ?? Model);
        }

        #region Methods

        /// <summary>
        ///     Resolves the layout template.
        /// </summary>
        /// <param name="name">The name of the layout template.</param>
        /// <returns>
        ///     An instance of <see cref="ITemplate" />.
        /// </returns>
        protected override ITemplate ResolveLayout(string name)
        {
            return TemplateService.Resolve(name, (T) currentModel);
        }

        #endregion
    }
}