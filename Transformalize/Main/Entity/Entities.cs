using System;
using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Main {
    public class Entities : List<Entity> {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// See whether this collection has an entity; checking for alias first, then name.
        /// </summary>
        /// <param name="nameOrAlias"></param>
        /// <returns></returns>
        public bool Has(string nameOrAlias) {
            return this.Any(e => e.Alias.Equals(nameOrAlias, IC) || e.Name.Equals(nameOrAlias));
        }

        /// <summary>
        /// Find the first entity in this collection, checking for alias first, then name.
        /// </summary>
        /// <param name="nameOrAlias"></param>
        /// <returns></returns>
        private Entity Find(string nameOrAlias) {
            return this.First(e => e.Alias.Equals(nameOrAlias, IC) || e.Name.Equals(nameOrAlias));
        }

        /// <summary>
        /// Tries to find first entity in this collection, checking for alias first, then name.
        /// </summary>
        /// <param name="nameOfAlias"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool TryFind(string nameOfAlias, out Entity entity) {
            if (Has(nameOfAlias)) {
                entity = Find(nameOfAlias);
                return true;
            }
            entity = null;
            return false;
        }
    }
}