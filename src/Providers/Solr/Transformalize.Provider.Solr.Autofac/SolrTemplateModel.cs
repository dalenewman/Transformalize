using System.Dynamic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Solr.Autofac {
   public class SolrTemplateModel {
      public IConnectionContext Context { get; set; }
      public Process Process { get; set; }
      public ExpandoObject Parameters { get; set; }
   }
}