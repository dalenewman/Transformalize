using NUnit.Framework;

namespace Transformalize.Test {
    [TestFixture]
    public class TestProcess {

        [Test]
        public void RunProcess() {
            var process = new TransformalizeProcess("Test");
            process.Execute();
        }

    }
}
