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
    /// A helper class that provides the code needed to wrap an existing
    /// asynchronous operation and return a different implementation of
    /// <see cref="IAsyncResult"/>.
    /// </summary>
    public static class WrappedAsyncOperation
    {
        /// <summary>
        /// Start an asyncronous operation that wraps a lower-level
        /// async operation.
        /// </summary>
        /// <typeparam name="TWrappingAsyncResult">Type that implements IAsyncResult
        /// that will be returned from this method.</typeparam>
        /// <param name="callback">The user's callback method to be called when
        /// the async operation completes.</param>
        /// <param name="beginOperation">A delegate that invokes the underlying
        /// async operation that we're wrapping.</param>
        /// <param name="wrappingResultCreator">A delegate that takes the inner
        /// async result and returns the wrapping instance of <typeparamref name="TWrappingAsyncResult"/>.
        /// </param>
        /// <returns>The <see cref="IAsyncResult"/>.</returns>
        public static IAsyncResult BeginAsyncOperation<TWrappingAsyncResult>(
            AsyncCallback callback,
            Func<AsyncCallback, IAsyncResult> beginOperation,
            Func<IAsyncResult, TWrappingAsyncResult> wrappingResultCreator)
            where TWrappingAsyncResult : class, IAsyncResult
        {
            // Implmentation here is a little complex, so a few comments are in order.
            // We have our custom AsyncResult object, which needs to be passed to
            // the callback. However, when we call SqlCommand.BeginExecuteReader, what
            // it passes to the callback is it's own IAsyncResult object, not ours.
            // We work around the issue by wrapping the user supplied callback in
            // another lambda function that has access to the DaabAsyncResult we want
            // passed in.
            // HOWEVER, there's a gotcha here. We can't create the DaabAsyncResult instance
            // until we have the inner IAsyncResult object. But we don't get that inner
            // object until BeginExecuteReader completes, so it's too late to pass it in
            // to the new callback. This is why we're using a closure here. The lock is
            // needed to assure we don't call the user callback until after the DaabAsyncResult
            // object has been properly created.
            // And ONE MORE GOTCHA: It's (theoretically) possible that BeginExecuteReader
            // may complete synchronously, and call the callback on the same thread before
            // returning the Async result. In that case, the lock won't help because we're
            // already on the right thread and recursive acquires are fine. So we also check
            // the CompletedSynchronously flag and handle the callback separately.

            var padlock = new object();
            TWrappingAsyncResult result = null;
            AsyncCallback wrapperCallback = null;
            if (callback != null)
            {
                wrapperCallback = ar =>
                {
                    if (!ar.CompletedSynchronously)
                    {
                        lock (padlock) { }
                        callback(result);
                    }
                };
            }

            lock (padlock)
            {
                IAsyncResult innerAsyncResult = beginOperation(wrapperCallback);
                result = wrappingResultCreator(innerAsyncResult);
            }

            if (result.CompletedSynchronously && callback != null)
            {
                callback(result);
            }
            return result;
        }
    }
}
