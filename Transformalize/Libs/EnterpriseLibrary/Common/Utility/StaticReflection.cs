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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Utility
{
    /// <summary>
    /// Provides a set of helper methods that retrieve
    /// <see cref="MethodInfo"/> objects for the methods described in lambda expressions.
    /// </summary>
    public static class StaticReflection
    {
        /// <summary>
        /// Retrieves a <see cref="MethodInfo"/> object from an expression in the form
        /// () => SomeClass.SomeMethod().
        /// </summary>
        /// <param name="expression">The expression that describes the method to call.</param>
        /// <returns>The <see cref="MethodInfo"/> object for the specified method.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
        /// Retrieves a <see cref="MethodInfo"/> object from an expression in the form
        /// x => x.SomeMethod().
        /// </summary>
        /// <typeparam name="T">The type where the method is defined.</typeparam>
        /// <param name="expression">The expression that describes the method to call.</param>
        /// <returns>The <see cref="MethodInfo"/> object for the specified method.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expressions require nested generics")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        private static MethodInfo GetMethodInfo(LambdaExpression lambda)
        {
            GuardProperExpressionForm(lambda.Body);

            var call = (MethodCallExpression)lambda.Body;
            return call.Method;
        }

        /// <summary>
        /// Retrieves a <see cref="MethodInfo"/> object for the get method from an expression in the form
        /// x => x.SomeProperty.
        /// </summary>
        /// <typeparam name="T">The type where the method is defined.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expression">The expression that describes the property for which the get method is to be extracted.</param>
        /// <returns>The <see cref="MethodInfo"/> object for the get method.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expressions require nested generics")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static MethodInfo GetPropertyGetMethodInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var property = GetPropertyInfo<T, TProperty>(expression);

            var getMethod = property.GetMethod;
            if (getMethod == null) throw new InvalidOperationException("Invalid expression form passed");

            return getMethod;
        }

        /// <summary>
        /// Retrieves a <see cref="MethodInfo"/> object for the set method from an expression in the form
        /// x => x.SomeProperty.
        /// </summary>
        /// <typeparam name="T">The type where the method is defined.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expression">The expression that describes the property for which the set method is to be extracted.</param>
        /// <returns>The <see cref="MethodInfo"/> object for the set method.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expressions require nested generics")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        public static MethodInfo GetPropertySetMethodInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var property = GetPropertyInfo<T, TProperty>(expression);

            var setMethod = property.SetMethod;
            if (setMethod == null) throw new InvalidOperationException("Invalid expression form passed");

            return setMethod;
        }

        private static PropertyInfo GetPropertyInfo<T, TProperty>(LambdaExpression lambda)
        {
            var body = lambda.Body as MemberExpression;
            if (body == null) throw new InvalidOperationException("Invalid expression form passed");

            var property = body.Member as PropertyInfo;
            if (property == null) throw new InvalidOperationException("Invalid expression form passed");

            return property;
        }

        /// <summary>
        /// Retrieves a <see cref="PropertyInfo"/> object for the set method from an expression in the form
        /// x => x.SomeProperty.
        /// </summary>
        /// <typeparam name="T">The type where the method is defined.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expression">The expression that describes the property for which the property informationis to be extracted.</param>
        /// <returns>The <see cref="PropertyInfo"/> object for the property.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expressions require nested generics")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "As designed for setting usage expectations")]
        public static MemberInfo GetMemberInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            Guard.ArgumentNotNull(expression, "expression");

            var body = expression.Body as MemberExpression;
            if (body == null) throw new InvalidOperationException("invalid expression form passed");

            var member = body.Member as MemberInfo;
            if (member == null) throw new InvalidOperationException("Invalid expression form passed");
            return member;

        }

        /// <summary>
        /// Retrieves a <see cref="ConstructorInfo"/> object from an expression in the form () => new SomeType().
        /// </summary>
        /// <typeparam name="T">The type where the constructor is defined.</typeparam>
        /// <param name="expression">The expression that calls the desired constructor.</param>
        /// <returns>The <see cref="ConstructorInfo"/> object for the constructor.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expressions require nested generics")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Lambda inference at the call site doesn't work without the derived type.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public static ConstructorInfo GetConstructorInfo<T>(Expression<Func<T>> expression)
        {
            Guard.ArgumentNotNull(expression, "expression");

            var body = expression.Body as NewExpression;
            if (body == null) throw new InvalidOperationException("Invalid expression form passed");

            return body.Constructor;
        }

        private static void GuardProperExpressionForm(Expression expression)
        {
            if (expression.NodeType != ExpressionType.Call)
            {
                throw new InvalidOperationException("Invalid expression form passed");
            }
        }
    }
}
