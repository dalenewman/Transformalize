#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2022 Dale Newman
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
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;

namespace Transformalize.Providers.Lucene {
   public class SearcherFactory : System.IDisposable {
      private readonly IndexReaderFactory _readerFactory;
      private IndexReader _reader;

      public SearcherFactory(IndexReaderFactory readerFactory) {
         _readerFactory = readerFactory;
      }

      public IndexSearcher Create() {
         _reader = _readerFactory.Create();
         return new IndexSearcher(_reader);
      }

      public void Dispose() {
         if(_reader != null) {
            _reader.Dispose();
         }
      }
   }
}
