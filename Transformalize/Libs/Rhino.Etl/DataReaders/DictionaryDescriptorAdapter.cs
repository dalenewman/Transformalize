#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;

namespace Transformalize.Libs.Rhino.Etl.DataReaders
{
    /// <summary>
    ///     Adapts a dictionary to a descriptor
    /// </summary>
    public class DictionaryDescriptorAdapter : Descriptor
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DictionaryDescriptorAdapter" /> class.
        /// </summary>
        /// <param name="pair">The pair.</param>
        public DictionaryDescriptorAdapter(KeyValuePair<string, Type> pair)
            : base(pair.Key, pair.Value)
        {
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public override object GetValue(object obj)
        {
            return ((Row) obj)[Name];
        }
    }
}