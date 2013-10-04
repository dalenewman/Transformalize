#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.ObjectModel;

namespace Transformalize.Libs.NLog.LogReceiverService
{
#if WCF_SUPPORTED
    using System.Runtime.Serialization;
#endif

    /// <summary>
    ///     List of strings annotated for more terse serialization.
    /// </summary>
#if WCF_SUPPORTED
    [CollectionDataContract(ItemName = "l", Namespace = LogReceiverServiceConfig.WebServiceNamespace)]
#endif
    public class StringCollection : Collection<string>
    {
    }
}