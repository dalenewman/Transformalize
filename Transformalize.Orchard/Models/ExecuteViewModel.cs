namespace Transformalize.Orchard.Models {
    public class ExecuteViewModel {
        public Main.Process[] Processes { get; set; }
        public bool DisplayLog { get; set; }
        public string[] Log { get; set; }
        public ExecuteViewModel() {
            Log = new string[0];
            Processes = new Main.Process[0];
        }
    }
}