using NUnit.Framework;
using Transformalize.Main;

namespace Transformalize.Test {

    [TestFixture]
    public class TestTemp {

        [Test]
        [Ignore("some temporary stuff")]
        public void OneOff() {

            const string recipe = @"C:\Code\mydd\mydd\tfl\recipe-orchard.xml";
            ProcessFactory.CreateSingle(recipe + "?Mode=init", new TestLogger()).ExecuteScaler();
            ProcessFactory.CreateSingle(recipe + "?Mode=first", new TestLogger()).ExecuteScaler();

            const string clas = @"C:\Code\mydd\mydd\tfl\class-orchard.xml";
            ProcessFactory.CreateSingle(clas + "?Mode=init", new TestLogger()).ExecuteScaler();
            ProcessFactory.CreateSingle(clas + "?Mode=first", new TestLogger()).ExecuteScaler();

            Assert.AreEqual(2, 2);

        }
    }
}