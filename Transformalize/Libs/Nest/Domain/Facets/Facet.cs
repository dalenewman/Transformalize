using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.Domain.Facets
{
    public interface IFacet
    {
    }

    [JsonObject]
    public interface IFacet<T> : IFacet where T : FacetItem
    {
        IEnumerable<T> Items { get; }
    }
    [JsonObject]
    public abstract class Facet : IFacet
    {
    }
}