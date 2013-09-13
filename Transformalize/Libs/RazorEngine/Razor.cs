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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using Transformalize.Libs.RazorEngine.Templating;

namespace Transformalize.Libs.RazorEngine
{
    /// <summary>
    ///     Provides quick access to template functions.
    /// </summary>
    public static class Razor
    {
        #region Fields

        private static ITemplateService _service = new TemplateService();
        private static readonly object _sync = new object();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the template service.
        /// </summary>
        private static ITemplateService TemplateService
        {
            get
            {
                lock (_sync)
                    return _service;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Compiles the specified template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="cacheName">The name of the template type in cache.</param>
        public static void Compile(string razorTemplate, string cacheName)
        {
            TemplateService.Compile(razorTemplate, null, cacheName);
        }

        /// <summary>
        ///     Compiles the specified template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="modelType">The model type.</param>
        /// <param name="cacheName">The name of the template type in cache.</param>
        public static void Compile(string razorTemplate, Type modelType, string cacheName)
        {
            TemplateService.Compile(razorTemplate, modelType, cacheName);
        }

        /// <summary>
        ///     Compiles the specified template.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="cacheName">The name of the template type in cache.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "We already provide a non-generic alternative.")]
        public static void Compile<T>(string razorTemplate, string cacheName)
        {
            TemplateService.Compile(razorTemplate, typeof (T), cacheName);
        }

        /// <summary>
        ///     Creates an instance of <see cref="ITemplate" /> from the specified string template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <returns>
        ///     An instance of <see cref="ITemplate" />.
        /// </returns>
        public static ITemplate CreateTemplate(string razorTemplate)
        {
            return TemplateService.CreateTemplate(razorTemplate, null, null);
        }

        /// <summary>
        ///     Creates an instance of <see cref="ITemplate{T}" /> from the specified string template.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="model">The model instance.</param>
        /// <returns>
        ///     An instance of <see cref="ITemplate{T}" />.
        /// </returns>
        public static ITemplate CreateTemplate<T>(string razorTemplate, T model)
        {
            return TemplateService.CreateTemplate(razorTemplate, null, model);
        }

        /// <summary>
        ///     Creates a set of templates from the specified string templates.
        /// </summary>
        /// <param name="razorTemplates">
        ///     The set of templates to create <see cref="ITemplate" /> instances for.
        /// </param>
        /// <param name="parallel">Flag to determine whether to create templates in parallel.</param>
        /// <returns>The enumerable set of template instances.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<ITemplate> CreateTemplates(IEnumerable<string> razorTemplates, bool parallel = false)
        {
            return TemplateService.CreateTemplates(razorTemplates, null, null, parallel);
        }

        /// <summary>
        ///     Creates a set of templates from the specified string templates and models.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplates">
        ///     The set of templates to create <see cref="ITemplate" /> instances for.
        /// </param>
        /// <param name="models">The set of models used to assign to templates.</param>
        /// <param name="parallel">Flag to determine whether to create templates in parallel.</param>
        /// <returns>The enumerable set of template instances.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<ITemplate> CreateTemplates<T>(IEnumerable<string> razorTemplates, IEnumerable<T> models, bool parallel = false)
        {
            var modelList = (from m in models select (object) m).ToList();
            return TemplateService.CreateTemplates(razorTemplates, null, modelList, parallel);
        }

        /// <summary>
        ///     Creates a <see cref="Type" /> that can be used to instantiate an instance of a template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <returns>
        ///     An instance of <see cref="Type" />.
        /// </returns>
        public static Type CreateTemplateType(string razorTemplate)
        {
            return TemplateService.CreateTemplateType(razorTemplate, null);
        }

        /// <summary>
        ///     Creates a <see cref="Type" /> that can be used to instantiate an instance of a template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="modelType">The model type.</param>
        /// <returns>
        ///     An instance of <see cref="Type" />.
        /// </returns>
        public static Type CreateTemplateType(string razorTemplate, Type modelType)
        {
            return TemplateService.CreateTemplateType(razorTemplate, modelType);
        }

        /// <summary>
        ///     Crates a set of template types from the specfied string templates.
        /// </summary>
        /// <param name="razorTemplates">
        ///     The set of templates to create <see cref="Type" /> instances for.
        /// </param>
        /// <param name="parallel">Flag to determine whether to create template types in parallel.</param>
        /// <returns>
        ///     The set of <see cref="Type" /> instances.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<Type> CreateTemplateTypes(IEnumerable<string> razorTemplates, bool parallel = false)
        {
            return TemplateService.CreateTemplateTypes(razorTemplates, null, parallel);
        }

        /// <summary>
        ///     Creates a set of template types from the specfied string templates.
        /// </summary>
        /// <param name="razorTemplates">
        ///     The set of templates to create <see cref="Type" /> instances for.
        /// </param>
        /// <param name="modelType">The model type.</param>
        /// <param name="parallel">Flag to determine whether to create template types in parallel.</param>
        /// <returns>
        ///     The set of <see cref="Type" /> instances.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<Type> CreateTemplateTypes(IEnumerable<string> razorTemplates, Type modelType, bool parallel = false)
        {
            var templates = razorTemplates.ToArray();
            var modelTypes = Enumerable.Repeat(modelType, templates.Count());
            return TemplateService.CreateTemplateTypes(templates, modelTypes, parallel);
        }

        /// <summary>
        ///     Gets an instance of the template using the cached compiled type, or compiles the template type
        ///     if it does not exist in the cache.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="cacheName">The name of the template type in the cache.</param>
        /// <returns>
        ///     An instance of <see cref="ITemplate" />.
        /// </returns>
        public static ITemplate GetTemplate(string razorTemplate, string cacheName)
        {
            return TemplateService.GetTemplate(razorTemplate, null, cacheName);
        }

        /// <summary>
        ///     Gets an instance of the template using the cached compiled type, or compiles the template type
        ///     if it does not exist in the cache.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="model">The model instance.</param>
        /// <param name="cacheName">The name of the template type in the cache.</param>
        /// <returns>
        ///     An instance of <see cref="ITemplate{T}" />.
        /// </returns>
        public static ITemplate GetTemplate<T>(string razorTemplate, T model, string cacheName)
        {
            return TemplateService.GetTemplate(razorTemplate, model, cacheName);
        }

        /// <summary>
        ///     Gets the set of template instances for the specified string templates. Cached templates will be considered
        ///     and if they do not exist, new types will be created and instantiated.
        /// </summary>
        /// <param name="razorTemplates">The set of templates to create.</param>
        /// <param name="cacheNames">The set of cache names.</param>
        /// <param name="parallel">Flag to determine whether to get the templates in parallel.</param>
        /// <returns>
        ///     The set of <see cref="ITemplate" /> instances.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<ITemplate> GetTemplates(IEnumerable<string> razorTemplates, IEnumerable<string> cacheNames, bool parallel = false)
        {
            return TemplateService.GetTemplates(razorTemplates, null, cacheNames, parallel);
        }

        /// <summary>
        ///     Gets the set of template instances for the specified string templates. Cached templates will be considered
        ///     and if they do not exist, new types will be created and instantiated.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplates">The set of templates to create.</param>
        /// <param name="models">The set of models.</param>
        /// <param name="cacheNames">The set of cache names.</param>
        /// <param name="parallel">Flag to determine whether to get the templates in parallel.</param>
        /// <returns>
        ///     The set of <see cref="ITemplate" /> instances.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<ITemplate> GetTemplates<T>(IEnumerable<string> razorTemplates, IEnumerable<T> models, IEnumerable<string> cacheNames, bool parallel = false)
        {
            var modelList = (from m in models select (object) m).ToList();
            return TemplateService.GetTemplates(razorTemplates, modelList, cacheNames, parallel);
        }

        /// <summary>
        ///     Parses and returns the result of the specified string template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <returns>The string result of the template.</returns>
        public static string Parse(string razorTemplate)
        {
            return TemplateService.Parse(razorTemplate, null, null, null);
        }

        /// <summary>
        ///     Parses and returns the result of the specified string template.
        ///     This method will provide a cache check to see if the compiled template type already exists and is valid.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="cacheName">The name of the template type in the cache or NULL if no caching is desired.</param>
        /// <returns>The string result of the template.</returns>
        public static string Parse(string razorTemplate, string cacheName)
        {
            return TemplateService.Parse(razorTemplate, null, null, cacheName);
        }

        /// <summary>
        ///     Parses and returns the result of the specified string template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="model">The model instance.</param>
        /// <returns>The string result of the template.</returns>
        public static string Parse(string razorTemplate, object model)
        {
            return TemplateService.Parse(razorTemplate, model, null, null);
        }

        /// <summary>
        ///     Parses and returns the result of the specified string template.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="model">The model instance.</param>
        /// <returns>The string result of the template.</returns>
        public static string Parse<T>(string razorTemplate, T model)
        {
            return TemplateService.Parse(razorTemplate, model, null, null);
        }

        /// <summary>
        ///     Parses and returns the result of the specified string template.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="model">The model instance.</param>
        /// <param name="cacheName">The name of the template type in the cache or NULL if no caching is desired.</param>
        /// <returns>The string result of the template.</returns>
        public static string Parse<T>(string razorTemplate, T model, string cacheName)
        {
            return TemplateService.Parse(razorTemplate, model, null, cacheName);
        }

        /// <summary>
        ///     Parses and returns the result of the specified string template.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="model">The model instance.</param>
        /// <param name="viewBag">The ViewBag contents or NULL for an initially empty ViewBag.</param>
        /// <param name="cacheName">The name of the template type in the cache or NULL if no caching is desired.</param>
        /// <returns>The string result of the template.</returns>
        public static string Parse<T>(string razorTemplate, T model, DynamicViewBag viewBag, string cacheName)
        {
            return TemplateService.Parse(razorTemplate, model, viewBag, cacheName);
        }

        /// <summary>
        ///     Parses and returns the result of the specified string template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="model">The model instance.</param>
        /// <param name="cacheName">The name of the template type in the cache or NULL if no caching is desired.</param>
        /// <returns>The string result of the template.</returns>
        public static string Parse(string razorTemplate, object model, string cacheName)
        {
            return TemplateService.Parse(razorTemplate, model, null, cacheName);
        }

        /// <summary>
        ///     Parses and returns the result of the specified string template.
        /// </summary>
        /// <param name="razorTemplate">The string template.</param>
        /// <param name="model">The model instance.</param>
        /// <param name="viewBag">The ViewBag contents or NULL for an initially empty ViewBag.</param>
        /// <param name="cacheName">The name of the template type in the cache or NULL if no caching is desired.</param>
        /// <returns>The string result of the template.</returns>
        public static string Parse(string razorTemplate, object model, DynamicViewBag viewBag, string cacheName)
        {
            return TemplateService.Parse(razorTemplate, model, viewBag, cacheName);
        }

        /// <summary>
        ///     Parses the template and merges with the many models provided.
        /// </summary>
        /// <param name="razorTemplate">The razor template.</param>
        /// <param name="models">
        ///     The set of models (must contain at least one model).
        /// </param>
        /// <param name="parallel">Flag to determine whether parsing in parallel.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany(string razorTemplate, IEnumerable<object> models, bool parallel = false)
        {
            if (models == null)
                throw new ArgumentException("Expected models list (this parameter may not be NULL).");

            var m = models.ToArray();

            if (!m.Any())
                throw new ArgumentException("Expected at least one entry in models list.");

            var razorTemplateList = Enumerable.Repeat(razorTemplate, m.Count()).ToList();
            return TemplateService.ParseMany(razorTemplateList, m, null, null, parallel);
        }

        /// <summary>
        ///     Parses the specified set of templates.
        /// </summary>
        /// <param name="razorTemplates">The set of string templates to parse.</param>
        /// <param name="parallel">Flag to determine whether parsing in templates.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany(IEnumerable<string> razorTemplates, bool parallel = false)
        {
            return TemplateService.ParseMany(razorTemplates, null, null, null, parallel);
        }

        /// <summary>
        ///     Parses the specified set of templates.
        /// </summary>
        /// <param name="razorTemplates">The set of string templates to parse.</param>
        /// <param name="models">
        ///     The set of models or NULL if no models exist for all templates.
        ///     Individual elements in this set may be NULL if no model exists for a specific template.
        /// </param>
        /// <param name="parallel">Flag to determine whether parsing in templates.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany(IEnumerable<string> razorTemplates, IEnumerable<object> models, bool parallel = false)
        {
            return TemplateService.ParseMany(razorTemplates, models, null, null, parallel);
        }

        /// <summary>
        ///     Parses the specified set of templates.
        /// </summary>
        /// <param name="razorTemplates">The set of string templates to parse.</param>
        /// <param name="cacheNames">
        ///     The set of cache names or NULL if no caching is desired for templates.
        ///     Individual elements in this set may be NULL if caching is not desired for a specific template.
        /// </param>
        /// <param name="parallel">Flag to determine whether parsing in templates.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany(IEnumerable<string> razorTemplates, IEnumerable<string> cacheNames, bool parallel = false)
        {
            return TemplateService.ParseMany(razorTemplates, null, null, cacheNames, parallel);
        }

        /// <summary>
        ///     Parses the specified set of templates.
        /// </summary>
        /// <param name="razorTemplates">The set of string templates to parse.</param>
        /// <param name="models">
        ///     The set of models or NULL if no models exist for all templates.
        ///     Individual elements in this set may be NULL if no model exists for a specific template.
        /// </param>
        /// <param name="cacheNames">
        ///     The set of cache names or NULL if no caching is desired for templates.
        ///     Individual elements in this set may be NULL if caching is not desired for a specific template.
        /// </param>
        /// <param name="parallel">Flag to determine whether parsing in templates.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany(IEnumerable<string> razorTemplates, IEnumerable<object> models, IEnumerable<string> cacheNames, bool parallel = false)
        {
            return TemplateService.ParseMany(razorTemplates, models, null, cacheNames, parallel);
        }

        /// <summary>
        ///     Parses the specified set of templates.
        /// </summary>
        /// <param name="razorTemplates">The set of string templates to parse.</param>
        /// <param name="models">
        ///     The set of models or NULL if no models exist for all templates.
        ///     Individual elements in this set may be NULL if no model exists for a specific template.
        /// </param>
        /// <param name="viewBags">
        ///     The set of initial ViewBag contents or NULL for an initially empty ViewBag for all templates.
        ///     Individual elements in this set may be NULL if an initially empty ViewBag is desired for a specific template.
        /// </param>
        /// <param name="cacheNames">
        ///     The set of cache names or NULL if no caching is desired for templates.
        ///     Individual elements in this set may be NULL if caching is not desired for a specific template.
        /// </param>
        /// <param name="parallel">Flag to determine whether parsing in templates.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany(IEnumerable<string> razorTemplates, IEnumerable<object> models, IEnumerable<DynamicViewBag> viewBags, IEnumerable<string> cacheNames, bool parallel = false)
        {
            return TemplateService.ParseMany(razorTemplates, models, viewBags, cacheNames, parallel);
        }

        /// <summary>
        ///     Parses the template and merges with the many models provided.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplate">The razor template.</param>
        /// <param name="models">
        ///     The set of models (must contain at least one model).
        /// </param>
        /// <param name="parallel">Flag to determine whether parsing in parallel.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany<T>(string razorTemplate, IEnumerable<T> models, bool parallel = false)
        {
            if (models == null)
                throw new ArgumentException("Expected models list (this parameter may not be NULL).");

            var ma = models.ToArray();

            if (!ma.Any())
                throw new ArgumentException("Expected at least one entry in models list.");

            var razorTemplateList = Enumerable.Repeat(razorTemplate, ma.Count()).ToList();
            var modelList = (from m in ma select (object) m).ToList();
            return TemplateService.ParseMany(razorTemplateList, modelList, null, null, parallel);
        }

        /// <summary>
        ///     Parses the specified set of templates.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplates">The set of string templates to parse.</param>
        /// <param name="models">
        ///     The set of models or NULL if no models exist for all templates.
        ///     Individual elements in this set may be NULL if no model exists for a specific template.
        /// </param>
        /// <param name="parallel">Flag to determine whether parsing in templates.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany<T>(IEnumerable<string> razorTemplates, IEnumerable<T> models, bool parallel = false)
        {
            var modelList = (from m in models select (object) m).ToList();
            return TemplateService.ParseMany(razorTemplates, modelList, null, null, parallel);
        }

        /// <summary>
        ///     Parses the specified set of templates.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplates">The set of string templates to parse.</param>
        /// <param name="models">
        ///     The set of models or NULL if no models exist for all templates.
        ///     Individual elements in this set may be NULL if no model exists for a specific template.
        /// </param>
        /// <param name="cacheNames">
        ///     The set of cache names or NULL if no caching is desired for templates.
        ///     Individual elements in this set may be NULL if caching is not desired for a specific template.
        /// </param>
        /// <param name="parallel">Flag to determine whether parsing in templates.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany<T>(IEnumerable<string> razorTemplates, IEnumerable<T> models, IEnumerable<string> cacheNames, bool parallel = false)
        {
            var modelList = (from m in models select (object) m).ToList();
            return TemplateService.ParseMany(razorTemplates, modelList, null, cacheNames, parallel);
        }

        /// <summary>
        ///     Parses the specified set of templates.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="razorTemplates">The set of string templates to parse.</param>
        /// <param name="models">
        ///     The set of models or NULL if no models exist for all templates.
        ///     Individual elements in this set may be NULL if no model exists for a specific template.
        /// </param>
        /// <param name="viewBags">
        ///     The set of initial ViewBag contents or NULL for an initially empty ViewBag for all templates.
        ///     Individual elements in this set may be NULL if an initially empty ViewBag is desired for a specific template.
        /// </param>
        /// <param name="cacheNames">
        ///     The set of cache names or NULL if no caching is desired for templates.
        ///     Individual elements in this set may be NULL if caching is not desired for a specific template.
        /// </param>
        /// <param name="parallel">Flag to determine whether parsing in templates.</param>
        /// <returns>The set of parsed template results.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IEnumerable<string> ParseMany<T>(IEnumerable<string> razorTemplates, IEnumerable<T> models, IEnumerable<DynamicViewBag> viewBags, IEnumerable<string> cacheNames, bool parallel = false)
        {
            var modelList = (from m in models select (object) m).ToList();
            return TemplateService.ParseMany(razorTemplates, modelList, viewBags, cacheNames, parallel);
        }

        /// <summary>
        ///     Resolves the template with the specified name.
        /// </summary>
        /// <param name="cacheName">The name of the template type in cache.</param>
        /// <returns>The resolved template.</returns>
        public static ITemplate Resolve(string cacheName)
        {
            return TemplateService.Resolve(cacheName, null);
        }

        /// <summary>
        ///     Resolves the template with the specified name.
        /// </summary>
        /// <param name="cacheName">The name of the template type in cache.</param>
        /// <param name="model">The model for the template.</param>
        /// <returns>The resolved template.</returns>
        public static ITemplate Resolve(string cacheName, object model)
        {
            return TemplateService.Resolve(cacheName, model);
        }

        /// <summary>
        ///     Resolves the template with the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheName">The name of the template type in cache.</param>
        /// <param name="model">The model for the template.</param>
        /// <returns>The resolved template.</returns>
        public static ITemplate Resolve<T>(string cacheName, T model)
        {
            return TemplateService.Resolve(cacheName, model);
        }

        /// <summary>
        ///     Runs the template with the specified name.
        /// </summary>
        /// <param name="cacheName">The name of the template type in cache.</param>
        /// <returns>The string result of the template.</returns>
        public static string Run(string cacheName)
        {
            return TemplateService.Run(cacheName, null, null);
        }

        /// <summary>
        ///     Runs the template with the specified name.
        /// </summary>
        /// <param name="cacheName">The name of the template type in cache.</param>
        /// <param name="model">The model.</param>
        /// <returns>The string result of the template.</returns>
        public static string Run(string cacheName, object model)
        {
            return TemplateService.Run(cacheName, model, null);
        }

        /// <summary>
        ///     Runs the template with the specified name.
        /// </summary>
        /// <param name="cacheName">The name of the template type in cache.</param>
        /// <param name="model">The model.</param>
        /// <param name="viewBag">The ViewBag contents or NULL for an initially empty ViewBag.</param>
        /// <returns>The string result of the template.</returns>
        public static string Run(string cacheName, object model, DynamicViewBag viewBag)
        {
            return TemplateService.Run(cacheName, model, viewBag);
        }

        /// <summary>
        ///     Runs the template with the specified name.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="cacheName">The name of the template type in cache.</param>
        /// <param name="model">The model.</param>
        /// <returns>The string result of the template.</returns>
        public static string Run<T>(string cacheName, T model)
        {
            return TemplateService.Run(cacheName, model, null);
        }

        /// <summary>
        ///     Runs the template with the specified name.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="cacheName">The name of the template type in cache.</param>
        /// <param name="model">The model.</param>
        /// <param name="viewBag">The ViewBag contents or NULL for an initially empty ViewBag.</param>
        /// <returns>The string result of the template.</returns>
        public static string Run<T>(string cacheName, T model, DynamicViewBag viewBag)
        {
            return TemplateService.Run(cacheName, model, viewBag);
        }

        /// <summary>
        ///     Sets the template service.
        /// </summary>
        /// <param name="service">The template service.</param>
        public static void SetTemplateService(ITemplateService service)
        {
            Contract.Requires(service != null);

            lock (_sync)
                _service = service;
        }

        #endregion
    }
}