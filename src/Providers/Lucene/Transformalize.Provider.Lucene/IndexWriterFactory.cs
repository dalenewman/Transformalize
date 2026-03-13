#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Transformalize.Providers.Lucene {

   /// <summary>
   /// A factory is needed here because we don't want to instantiate the index reader at composition root.
   /// This factory will create an IndexReader and apply default locks when it does.
   /// </summary>
   public class IndexWriterFactory {
      private readonly DirectoryFactory _directoryFactory;
      private readonly IndexWriterConfig _config;

      public IndexWriterFactory(DirectoryFactory directoryFactory, Analyzer analyzer) {
         _directoryFactory = directoryFactory;
         _config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
      }

      /// <summary>
      /// Always create and use in `using` construct.  This must be disposed of.
      /// </summary>
      /// <returns></returns>
      public IndexWriter Create() {
         return new IndexWriter(_directoryFactory.Create(), _config);
      }
   }
}
