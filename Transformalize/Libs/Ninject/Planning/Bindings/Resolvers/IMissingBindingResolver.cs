#region License
// /*
// See license included in this library folder.
// */
#endregion
#region Using Directives

using System;
using System.Collections.Generic;
using Transformalize.Libs.Ninject.Activation;
using Transformalize.Libs.Ninject.Components;
using Transformalize.Libs.Ninject.Infrastructure;

#endregion

namespace Transformalize.Libs.Ninject.Planning.Bindings.Resolvers
{
    /// <summary>
    ///     Contains logic about which bindings to use for a given service request
    ///     when other attempts have failed.
    /// </summary>
    public interface IMissingBindingResolver : INinjectComponent
    {
        /// <summary>
        ///     Returns any bindings from the specified collection that match the specified request.
        /// </summary>
        /// <param name="bindings">The multimap of all registered bindings.</param>
        /// <param name="request">The request in question.</param>
        /// <returns>The series of matching bindings.</returns>
        IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request);
    }
}