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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Utility
{
    /// <summary>
    /// Extensios to <see cref="Type"/>
    /// </summary>
    public static class TypeExtensions
    {
        ///<summary>
        /// Locates the generic parent of the type
        ///</summary>
        ///<param name="rootType">Type to begin search from.</param>
        ///<param name="parentType">Open generic type to seek</param>
        ///<returns>The found parent that is a closed generic of the <paramref name="parentType"/> or null</returns>
        public static Type FindGenericParent(this Type rootType, Type parentType)
        {
            if (parentType == null) throw new ArgumentNullException("parentType");
            if (rootType == null) throw new ArgumentNullException("rootType");

            if (!parentType.IsGenericType) return null;

            Type currentType = rootType;
            while (currentType != typeof(object))
            {
                if (!currentType.IsGenericType)
                {
                    currentType = currentType.BaseType;
                    continue;
                }

                var genericType = currentType.GetGenericTypeDefinition();
                if (genericType == parentType) return currentType;

                currentType = currentType.BaseType;
            }

            return null;
        }
    }
}
