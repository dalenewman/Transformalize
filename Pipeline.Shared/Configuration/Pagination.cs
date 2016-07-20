using System;

namespace Pipeline.Configuration {

    public class Pagination {
        public int Pages { get; }
        public bool HasPrevious { get; private set; }
        public bool HasNext { get; private set; }
        public int Previous { get; private set; }
        public int Next { get; private set; }
        public int First { get; private set; }
        public int Last { get; private set; }

        public Pagination(int hits, int page, int pageSize) {
            var pages = (int)Math.Ceiling((decimal)hits / pageSize);
            Pages = pages;
            HasPrevious = page > 1;
            Previous = page == 1 ? 1 : page - 1;
            HasNext = page < pages;
            Next = page == pages ? page : page + 1;
            Last = Pages;
            First = 1;
        }
    }
}
