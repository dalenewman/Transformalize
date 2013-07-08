using System.Collections.Generic;

namespace Transformalize.Model {

    public class Relationship {
        public Entity LeftEntity;
        public Entity RightEntity;
        public List<Join> Join;

        public Relationship() {
            Join = new List<Join>();
        }
    }
}