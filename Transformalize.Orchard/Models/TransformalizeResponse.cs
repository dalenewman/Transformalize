using System.Collections.Generic;
using Transformalize.Main;

namespace Transformalize.Orchard.Models {
    public class TransformalizeResponse {
        public Process[] Processes { get; set; }
        public List<LinkedList<string>> Log { get; set; }

        public TransformalizeResponse() {
            Processes = new Process[0];
            Log = new List<LinkedList<string>>();
        }
    }
}