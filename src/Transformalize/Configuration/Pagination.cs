#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System;

namespace Transformalize.Configuration {

   public class Pagination {
      public int Page { get; set; }
      public int PageSize { get; set; }
      public int Hits { get; set; }
      public int Pages { get; }
      public bool HasPrevious { get; private set; }
      public bool HasNext { get; private set; }
      public int Previous { get; private set; }
      public int Next { get; private set; }
      public int First { get; private set; }
      public int Last { get; private set; }
      public int Size { get; set; }
      public int StartRow { get; set; }
      public int EndRow { get; set; }

      public Pagination(int hits, int page, int pageSize) {

         Hits = hits;
         Page = page;
         PageSize = pageSize;

         Pages = PageSize == 0 ? 0 : (int)Math.Ceiling((decimal)Hits / PageSize);

         HasPrevious = Page > 1;
         Previous = Page == 1 ? 1 : Page - 1;
         HasNext = Page < Pages;
         Next = Page == Pages ? Page : Page + 1;

         First = 1;
         Last = Pages;

         StartRow = (Page * PageSize) - PageSize + 1;

         if (PageSize > 0 && Hits > 0) {
            if (Page == Last) {
               Size = Hits - StartRow + 1;
            } else {
               if (Hits < PageSize) {
                  Size = PageSize - Hits;
               } else {
                  Size = PageSize;
               }
            }
         }
         
         EndRow = StartRow + Size - 1;
      }
   }
}
